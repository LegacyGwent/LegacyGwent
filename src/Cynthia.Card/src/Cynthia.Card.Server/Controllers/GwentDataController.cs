using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cynthia.Card.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GwentDataController : ControllerBase
    {
        private GwentDatabaseService _databaseService;
        private GwentServerService _gwentServerService;

        public GwentDataController(GwentDatabaseService databaseService, GwentServerService gwentServerService)
        {
            _databaseService = databaseService;
            _gwentServerService = gwentServerService;
        }

        [Route("{time:DateTime}")]
        public IEnumerable<GameResult> GetGameResults(DateTime time)
        {
            return _databaseService.GetGameResults(time);
        }

        [Route("{time:DateTime}")]
        public string QueryEnvironment(DateTime time)
        {
            return _databaseService.QueryEnvironment(time);
        }

        [Route("{time:DateTime}")]
        public string QueryMatches(DateTime time)
        {
            return _databaseService.QueryMatches(time);
        }

        public string OnlineInfo()
        {
            var info = _gwentServerService.GetUsers();
            var json = new
            {
                Users = info.Item1.Select(user => new { UserState = user.Key, Users = user.Select(x => x.PlayerName).ToList() }),
                Player = info.Item2.Select(p => new { Player1 = p.Item1, Player2 = p.Item2 }),
                AiPlayer = info.Item3.Select(p => new
                {
                    Player1 = p.Item1,
                    Player2 = p.Item2
                }),
            };
            return json.ToJson();
        }

        public int OnlineCount()
        {
            return _gwentServerService.GetUserCount();
        }

        [Route("{time:datetime}/{rankedOnly:bool?}")]
        public string QueryCard(DateTime time, bool? rankedOnly)
        {
            return _databaseService.QueryCard(time, rankedOnly);
        }



        [Route("{time:datetime}/{rankedOnly:bool?}")]
        public string QueryRanking(DateTime time, bool? rankedOnly)
        {
            return _databaseService.QueryRanking(time, rankedOnly);
        }

        [HttpPost]
        public async Task<IActionResult> AwardTrinketToUsers([FromBody] AwardTrinketRequest request)
        {
            if (request == null || request.Usernames == null || request.Usernames.Count == 0 || string.IsNullOrEmpty(request.TrinketId))
            {
                return BadRequest("Invalid request. Usernames list and TrinketId are required.");
            }

            var results = new List<object>();
            var successCount = 0;
            var failureCount = 0;

            foreach (var username in request.Usernames)
            {
                try
                {
                    bool result = false;
                    
                    switch (request.TrinketType)
                    {
                        case TrinketType.Avatar:
                            result = await _gwentServerService.AddAvatar(username, request.TrinketId);
                            break;
                        case TrinketType.Title:
                            result = await _gwentServerService.AddTitle(username, request.TrinketId);
                            break;
                        case TrinketType.Border:
                            result = await _gwentServerService.AddBorder(username, request.TrinketId);
                            break;
                        default:
                            results.Add(new { Username = username, Status = "Error", Message = "Invalid trinket type" });
                            failureCount++;
                            continue;
                    }

                    if (result)
                    {
                        successCount++;
                        results.Add(new { Username = username, Status = "Success" });
                    }
                    else
                    {
                        failureCount++;
                        results.Add(new { Username = username, Status = "Failed" });
                    }
                }
                catch (Exception ex)
                {
                    failureCount++;
                    results.Add(new { Username = username, Status = "Error", Message = ex.Message });
                }
            }

            var response = new
            {
                TotalUsers = request.Usernames.Count,
                SuccessCount = successCount,
                FailureCount = failureCount,
                TrinketType = request.TrinketType.ToString(),
                TrinketId = request.TrinketId,
                Results = results
            };

            return Ok(response);
        }

        // public IEnumerable<GameResult> GetAllGameResults()
        // {
        //     return _databaseService.GetGameResults(DateTime.Now.AddYears(-10));
        // }

        // public string QueryAllEnvironment()
        // {
        //     return _databaseService.QueryEnvironment(DateTime.Now.AddYears(-10));
        // }

        // public string QueryAllMatches()
        // {
        //     return _databaseService.QueryMatches(DateTime.Now.AddYears(-10));
        // }

        // public string QueryAllCard()
        // {
        //     return _databaseService.QueryCard(DateTime.Now.AddYears(-10));
        // }

        // public string QueryAllRanking()
        // {
        //     return _databaseService.QueryRanking(DateTime.Now.AddYears(-10));
        // }
    }
}