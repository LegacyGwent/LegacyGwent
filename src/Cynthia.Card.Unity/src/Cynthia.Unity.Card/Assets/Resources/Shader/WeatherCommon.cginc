// Shared helpers for the Weather/* row effect shaders.
// The effects are rebuilt from the original Gwent board effects: same textures, colours and layer order,
// with the separate quads and particle systems of the original folded into a single UI shader per weather.
// Shaders accumulate premultiplied colour so alpha-blended and additive layers can be mixed in one pass
// (Blend One OneMinusSrcAlpha).
#ifndef WEATHER_COMMON_INCLUDED
#define WEATHER_COMMON_INCLUDED

#include "UnityCG.cginc"

// Board rows are about 7.6 times wider than they are tall; multiply uv.x by this for round shapes.
#define ROW_ASPECT 7.6

float _Progress;
float _TimeOffset;
// Per-row variation: every row gets its own random seed (0..1) and a random mirror (0 or 1), like the
// original where each row's effect was a separate instance with its own random particles.
float _Seed;
float _Flip;
// The effect image is larger than the row (by _Pad of the row size on each side) so effects can spill over its edge.
float4 _Pad;

struct appdata_t
{
    float4 vertex : POSITION;
    float4 color : COLOR;
    float2 texcoord : TEXCOORD0;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    fixed4 color : COLOR;
    float2 uv : TEXCOORD0;
};

v2f vert(appdata_t IN)
{
    v2f OUT;
    OUT.vertex = UnityObjectToClipPos(IN.vertex);
    // uv in row space: 0..1 is the row itself, the padding lies outside that range.
    OUT.uv = IN.texcoord * (1 + 2 * _Pad.xy) - _Pad.xy;
    OUT.color = IN.color;
    return OUT;
}

float weatherTime()
{
    return _Time.y + _TimeOffset + _Seed * 100;
}

float hash(float2 p)
{
    return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
}

float2 hash2(float2 p)
{
    return frac(sin(float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)))) * 43758.5453);
}

float noise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);
    f = f * f * (3 - 2 * f);
    return lerp(lerp(hash(i), hash(i + float2(1, 0)), f.x),
                lerp(hash(i + float2(0, 1)), hash(i + float2(1, 1)), f.x), f.y);
}

// Premultiplied accumulation: alpha-blend a layer on top.
void layerOver(inout float4 acc, float3 rgb, float a)
{
    a = saturate(a);
    acc.rgb = rgb * a + acc.rgb * (1 - a);
    acc.a = a + acc.a * (1 - a);
}

// Premultiplied accumulation: add light without covering what is below.
void layerAdd(inout float4 acc, float3 rgb)
{
    acc.rgb += max(rgb, 0);
}

// Darken what is below (the original's multiply layers), expressed as a black layer.
void layerDarken(inout float4 acc, float amount)
{
    layerOver(acc, float3(0, 0, 0), amount);
}

// Samples one frame of a flipbook sheet (frames left to right, top to bottom).
float4 flipbook(sampler2D tex, float2 uv, float2 tiles, float frame)
{
    frame = floor(fmod(frame, tiles.x * tiles.y));
    float2 cell = float2(fmod(frame, tiles.x), tiles.y - 1 - floor(frame / tiles.x));
    return tex2D(tex, (cell + saturate(uv)) / tiles);
}

// Fades a sprite out towards its border, so textures that do not reach zero at their edge show no seam.
float softWindow(float2 local)
{
    float2 w = smoothstep(0, 0.12, local) * smoothstep(1, 0.88, local);
    return w.x * w.y;
}

float4 sprite(sampler2D tex, float2 uv, float2 centre, float2 size)
{
    float2 local = (uv - centre) / size + 0.5;
    return tex2D(tex, saturate(local)) * softWindow(local);
}

// Same for a flipbook frame.
float4 spriteFlipbook(sampler2D tex, float2 uv, float2 centre, float2 size, float2 tiles, float frame)
{
    float2 local = (uv - centre) / size + 0.5;
    return flipbook(tex, local, tiles, frame) * softWindow(local);
}

// The original reveals every effect with a soft gradient sliding across the row (sharp_side_gradient_alpha).
// 1 behind the front, 0 ahead of it; fully revealed at progress 1. A little noise keeps the front ragged.
float sweepRevealAt(float2 uv, float progress, float width)
{
    float front = progress * (1 + width * 2) - width;
    float x = uv.x + (noise(float2(uv.y * 6, progress * 3 + _Seed * 10)) - 0.5) * width * 0.6;
    return smoothstep(front + width * 0.5, front - width * 0.5, x);
}

float sweepReveal(float2 uv, float width)
{
    return sweepRevealAt(uv, _Progress, width);
}

// Position of the sweep front, and how strongly the front glows (only while it moves).
float sweepFront(float width)
{
    return _Progress * (1 + width * 2) - width;
}

float sweepActive()
{
    return sin(saturate(_Progress) * 3.14159);
}

// Maps row uv into the local 0..1 space of one of the original effect quads (centre/size in row uv).
// Most original effects are mirrored horizontally; flipX reproduces that, so textures scroll the same way.
float2 quadLocal(float2 uv, float2 centre, float2 size, float flipX)
{
    float2 l = (uv - centre) / size + 0.5;
    l.x = lerp(l.x, 1 - l.x, flipX);
    return l;
}

// Same as quadLocal, but also mirrored per row (_Flip) for static textures, so neighbouring rows don't look identical.
float2 quadLocalVaried(float2 uv, float2 centre, float2 size, float flipX)
{
    return quadLocal(uv, centre, size, abs(flipX - _Flip));
}

float insideQuad(float2 l)
{
    return step(0, l.x) * step(l.x, 1) * step(0, l.y) * step(l.y, 1);
}

// Soft fade towards the border of a quad (local 0..1), so no layer ever ends in a hard edge.
float edgeFade(float2 l, float2 soft)
{
    float2 a = smoothstep(0, soft, l) * smoothstep(1, 1 - soft, l);
    return a.x * a.y;
}

// Everything that spills over the row edge fades out smoothly across the padding, so nothing is cut off.
float padFade(float2 uv)
{
    float2 inner = _Pad.xy * 0.1;
    float2 a = smoothstep(-_Pad.xy, -inner, uv) * smoothstep(1 + _Pad.xy, 1 + inner, uv);
    return a.x * a.y;
}

// Row-sized layers: fade out just past the row edge.
float rowFade(float2 uv)
{
    return edgeFade((uv - float2(-0.02, -0.12)) / float2(1.04, 1.24), float2(0.03, 0.12));
}

// A particle emitter folded into the shader. Each slot has its own random period, so particles do not
// appear in a regular rhythm. Returns 1 while the slot's particle is alive; life runs 0..1 over its lifetime.
float particle(float slot, float t, float lifetime, float spacing, out float2 seed, out float life)
{
    slot += _Seed * 113;
    float period = lifetime * (1 + spacing) * (0.75 + 0.5 * hash(float2(slot, 1.7)));
    float cycle = t / period + hash(float2(slot, 7.13));
    seed = hash2(float2(slot * 3.7, floor(cycle)));
    // A particle never outlives its slot's period, otherwise it would be cut off mid-life.
    float len = min(lifetime * (0.8 + 0.4 * hash(seed + 0.5)), period);
    life = frac(cycle) * period / len;
    return step(life, 1);
}

// Smooth fade in and out over a particle's life, so particles never pop.
float envelope(float life, float fadeIn, float fadeOut)
{
    return smoothstep(0, fadeIn, life) * smoothstep(1, 1 - fadeOut, life);
}


// ---- Particle helpers modelled on the original Unity particle systems ----

// Original effects are measured in world units: a row is about 180 x 22 units.
#define UNITS_X (1.0 / 180.0)
#define UNITS_Y (1.0 / 22.0)

float2 rotate2(float2 p, float a)
{
    float s = sin(a);
    float c = cos(a);
    return float2(c * p.x - s * p.y, s * p.x + c * p.y);
}

// Local 0..1 coordinates of a square, rotated particle; size is in row heights.
float2 particleLocal(float2 uv, float2 centre, float size, float angle)
{
    float2 d = (uv - centre) * float2(ROW_ASPECT, 1);
    return rotate2(d, -angle) / max(size, 0.0001) + 0.5;
}

float4 particleSprite(sampler2D tex, float2 uv, float2 centre, float size, float angle)
{
    float2 l = particleLocal(uv, centre, size, angle);
    return tex2D(tex, saturate(l)) * softWindow(l);
}

float4 particleFlipbook(sampler2D tex, float2 uv, float2 centre, float size, float angle, float2 tiles, float frame)
{
    float2 l = particleLocal(uv, centre, size, angle);
    return flipbook(tex, l, tiles, frame) * softWindow(l);
}

// Flipbook with a soft cross-fade between frames, so slow sheets never pop.
float4 particleFlipbookBlend(sampler2D tex, float2 uv, float2 centre, float size, float angle, float2 tiles, float frame)
{
    float2 l = particleLocal(uv, centre, size, angle);
    float4 a = flipbook(tex, l, tiles, floor(frame));
    float4 b = flipbook(tex, l, tiles, min(floor(frame) + 1, tiles.x * tiles.y - 1));
    return lerp(a, b, frac(frame)) * softWindow(l);
}

// Stretched billboard: a soft streak from the particle back along its velocity (both in row uv).
float streak(float2 uv, float2 pos, float2 velocity, float lengthScale, float width)
{
    float2 p = (uv - pos) * float2(ROW_ASPECT, 1);
    float2 v = velocity * float2(ROW_ASPECT, 1) * lengthScale;
    float len2 = max(dot(v, v), 1e-6);
    float h = saturate(dot(p, -v) / len2);
    float d = length(p + v * h);
    return smoothstep(width, 0, d) * (1 - h * 0.7);
}

// Piecewise-linear curve through four keys (times t, values v), like a Unity AnimationCurve.
float curve4(float x, float4 t, float4 v)
{
    float r = v.x;
    r = lerp(r, v.y, saturate((x - t.x) / max(t.y - t.x, 1e-4)));
    r = lerp(r, v.z, saturate((x - t.y) / max(t.z - t.y, 1e-4)));
    r = lerp(r, v.w, saturate((x - t.z) / max(t.w - t.z, 1e-4)));
    return r;
}

// Seven-key curve as used by the original twinkle / flicker gradients (keys k0..k6 at times t0..t6).
float curve7(float x, float4 tA, float4 vA, float3 tB, float3 vB)
{
    return x < tA.w ? curve4(x, tA, vA) : curve4(x, float4(tA.w, tB), float4(vA.w, vB));
}

// A particle slot with a fixed lifetime: alive while seconds < lifetime, then the slot rests for a random gap.
float particleSeconds(float slot, float t, float lifetime, float gap, out float2 seed, out float seconds)
{
    slot += _Seed * 113;
    float period = lifetime + gap * (0.25 + hash(float2(slot, 1.7)) * 1.5);
    float cycle = t / period + hash(float2(slot, 7.13));
    seed = hash2(float2(slot * 3.7, floor(cycle)));
    seconds = frac(cycle) * period;
    return step(seconds, lifetime);
}

// Seconds since the weather was applied (set by WeatherRowEffect), for one-off intro particles.
float _AppliedAt;

float sinceApplied()
{
    return _Time.y - _AppliedAt;
}

// Extra random numbers for a particle.
float rand(float2 seed, float k)
{
    return hash(seed * 1.37 + k * 17.71);
}

#endif
