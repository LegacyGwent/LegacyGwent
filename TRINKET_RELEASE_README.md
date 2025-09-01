# Trinket Release Guide

This guide covers the complete process for developing and releasing new trinkets (avatars, titles, and borders) in the Gwent game.

## Overview

Trinkets are cosmetic items that enhance player experience. The release process involves:
1. **Development**: Creating trinket assets and code
2. **Configuration**: Adding trinkets to the game systems
3. **Localization**: Adding translations
4. **Testing**: Verifying functionality
5. **Deployment**: Releasing to production

## Development Workflow

### Phase 1: Asset Creation

#### Avatar Assets

- **File Format**: PNG with transparent backgrounds
- **Resolution**: 512x512 pixels recommended
- **Naming Convention**: `{CharacterName}_{TauntNumber}.png`

#### Border Assets
- **File Format**: PNG with transparency
- **Resolution**: Match UI frame dimensions
- **Naming Convention**: `{BorderName}.png`

#### Title Assets
- **Text Rendering**: Titles are text-based, no assets needed
- **Color Schemes**: Use predefined colors from ColorMap.cs

### Phase 2: Code Implementation

#### 1. Add to TrinketMap.cs

**File**: `src/Cynthia.Card/src/Cynthia.Card.Common/GwentGame/TrinketMap.cs`

##### New Avatar Example
```csharp
{
    "NewCharacter",
    new TrinketAvatar()
    {
        ID = "NewCharacter",
        Taunt1 = "NewCharacter1",
        Taunt2 = "NewCharacter2",
        Taunt3 = "NewCharacter3",
        Taunt4 = "NewCharacter4",
        Taunt5 = "NewCharacter5",
        Taunt6 = "NewCharacter6",
        IsReleased = false, // Set to false during development
    }
}
```

##### New Title Example
```csharp
{
    "NEW_TITLE",
    new Title()
    {
        ID = "NEW_TITLE",
        IsReleased = false,
        TitleColor = "emerald", // Choose from ColorMap.cs
    }
}
```

##### New Border Example
```csharp
{
    "NewBorder",
    new Border()
    {
        ID = "NewBorder",
        IsReleased = false,
    }
}
```

#### 2. Version Management

**IMPORTANT**: Never bump `TrinketMapVersion` during development. Only bump when releasing.

```csharp
// During development - DO NOT CHANGE
public static Version TrinketMapVersion { get; } = new Version(1, 0, 0, 5);

// When releasing - BUMP THIS
public static Version TrinketMapVersion { get; } = new Version(1, 0, 0, 6);
```

### Phase 3: Localization

Add translations to all locale files:

**Files**:
- `src/Cynthia.Card/src/Cynthia.Card.Server/Locales/en.json`
- `src/Cynthia.Card/src/Cynthia.Card.Server/Locales/cn.json`
- `src/Cynthia.Card/src/Cynthia.Card.Server/Locales/pl.json`
- `src/Cynthia.Card/src/Cynthia.Card.Server/Locales/ru.json`

#### Translation Format
```json
{
    "NewCharacterName": "New Character",
    "NewCharacterDescription": "A mysterious new character",
    "NEW_TITLEName": "NEW TITLE",
    "NEW_TITLEDescription": "A prestigious new title",
    "NewBorderName": "New Border",
    "NewBorderDescription": "An elegant new border"
}
```

### Phase 4: Testing

#### Development Testing
1. **Asset Loading**: Verify assets load correctly in Unity
2. **Code Compilation**: Ensure no compilation errors
3. **Local Testing**: Test trinkets in local development environment
4. **Translation Testing**: Verify all languages display correctly

#### Testing Checklist
- [ ] Assets display correctly in trinkets menu
- [ ] Taunt animations work for avatars
- [ ] Title colors render properly
- [ ] Translations appear in all languages
- [ ] No console errors or warnings
- [ ] Trinkets can be awarded via API (with `IsReleased = true`)

## Release Process

### Pre-Release Checklist

Before releasing, ensure:
- [ ] All assets are finalized and optimized
- [ ] Code is tested and working
- [ ] Translations are complete and accurate
- [ ] No `IsReleased = false` trinkets in production code
- [ ] Version bump is ready

### Step 1: Finalize Development

1. **Set IsReleased to true**:
```csharp
IsReleased = true, // Change from false to true
```

2. **Bump TrinketMapVersion**:
```csharp
public static Version TrinketMapVersion { get; } = new Version(1, 0, 0, 6);
```

3. **Commit Changes**:
```bash
git add .
git commit -m "Release new trinkets: NewCharacter, NEW_TITLE, NewBorder"
```

### Step 2: Deploy to Production

#### Server Deployment
1. **Build and Deploy Server**:
```bash
cd src/Cynthia.Card/src/Cynthia.Card.Server
dotnet build
dotnet publish -c Release
```

2. **Restart Server**: Ensure new TrinketMap version is loaded

#### Client Deployment
1. **Build Unity Project**: Create new client build
2. **Update Assets**: Ensure new trinket assets are included
3. **Distribute**: Release new client version

### Step 3: Post-Release Verification

#### Server Verification
1. **Check Logs**: Verify TrinketMap version is updated
2. **Test API**: Award trinkets to test users
3. **Monitor Errors**: Watch for any issues

#### Client Verification
1. **Test Trinkets Menu**: Verify new trinkets appear
2. **Check Translations**: Ensure all languages work
3. **User Testing**: Have players test new trinkets

## Release Strategies

### Feature Release
Release multiple trinkets together:
- **Advantages**: Efficient deployment, coordinated marketing
- **Best For**: Seasonal events, major updates

### Individual Release
Release trinkets one at a time:
- **Advantages**: Faster iteration, easier debugging
- **Best For**: Special events, quick fixes

### Staged Release
Release to subset of users first:
- **Advantages**: Risk mitigation, gradual rollout
- **Best For**: Major changes, complex trinkets

## Quality Assurance

### Code Review
- [ ] TrinketMap.cs changes reviewed
- [ ] Version bump is correct
- [ ] No syntax errors
- [ ] Follows naming conventions

### Asset Review
- [ ] Image quality meets standards
- [ ] File sizes are optimized
- [ ] Naming follows conventions
- [ ] Transparency handled correctly

### Localization Review
- [ ] All languages translated
- [ ] Grammar and spelling correct
- [ ] Cultural appropriateness
- [ ] Consistent terminology

## Troubleshooting

### Common Release Issues

#### Issue: Trinkets not appearing after release
**Causes**:
- `IsReleased = false` still in code
- Version not bumped
- Server not restarted
- Client not updated

**Solutions**:
1. Verify `IsReleased = true`
2. Check TrinketMapVersion
3. Restart server
4. Update client

#### Issue: Assets not loading
**Causes**:
- Missing asset files
- Incorrect file paths
- Build not including assets

**Solutions**:
1. Check asset files exist
2. Verify file paths in Unity
3. Rebuild client

#### Issue: Translations missing
**Causes**:
- Missing translation keys
- JSON syntax errors
- Locale files not updated

**Solutions**:
1. Check all locale files
2. Verify JSON syntax
3. Test all languages

## Best Practices

### Development
1. **Use Descriptive IDs**: Make trinket IDs meaningful
2. **Follow Naming Conventions**: Consistent naming across all files
3. **Test Early**: Test trinkets during development
4. **Document Changes**: Keep track of what's being added

### Release
1. **Version Control**: Always bump version when releasing
2. **Rollback Plan**: Have backup plan for failed releases
3. **Communication**: Notify team of release schedule
4. **Monitoring**: Watch for issues after release

### Maintenance
1. **Regular Reviews**: Periodically review trinket quality
2. **User Feedback**: Collect and act on user feedback
3. **Performance**: Monitor impact on game performance
4. **Documentation**: Keep documentation updated

## File Structure

```
src/Cynthia.Card/src/Cynthia.Card.Common/GwentGame/TrinketMap.cs
├── TrinketMapVersion (bump on release)
├── AvatarMap (add new avatars)
├── BorderMap (add new borders)
└── TitleMap (add new titles)

src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/
├── Textures/Avatars/ (avatar assets)
├── Textures/Borders/ (border assets)
└── Code/ColorMap.cs (title colors)

src/Cynthia.Card/src/Cynthia.Card.Server/Locales/
├── en.json (English)
├── cn.json (Chinese)
├── pl.json (Polish)
└── ru.json (Russian)
```

## Release Timeline Example

### Week 1: Development
- Create trinket assets
- Add to TrinketMap.cs (IsReleased = false)
- Add translations
- Local testing

### Week 2: Testing
- Comprehensive testing
- Bug fixes
- Final asset optimization
- Code review

### Week 3: Release
- Set IsReleased = true
- Bump TrinketMapVersion
- Deploy to production
- Monitor and verify

### Week 4: Post-Release
- Collect user feedback
- Monitor for issues
- Plan next release

This completes the trinket release workflow!
