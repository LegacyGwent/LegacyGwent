using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

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
                user = info.Item1,
                player = info.Item2,
                aiplayer = info.Item3,
            };
            return json.ToJson();
        }

        [Route("{time:DateTime}")]
        public string QueryCard(DateTime time)
        {
            return _databaseService.QueryCard(time);
        }

        [Route("{time:DateTime}")]
        public string QueryRanking(DateTime time)
        {
            return _databaseService.QueryRanking(time);
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