using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Cynthia.Card.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/{time:DateTime}")]
    public class GwentDataController : ControllerBase
    {
        private GwentDatabaseService _databaseService;

        public GwentDataController(GwentDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public IEnumerable<GameResult> GetGameResults(DateTime time)
        {
            return _databaseService.GetGameResults(time);
        }

        public string QueryEnvironment(DateTime time)
        {
            return _databaseService.QueryEnvironment(time);
        }

        public string QueryMatches(DateTime time)
        {
            return _databaseService.QueryMatches(time);
        }

        public string QueryCard(DateTime time)
        {
            return _databaseService.QueryCard(time);
        }

        public string QueryRanking(DateTime time)
        {
            return _databaseService.QueryRanking(time);
        }

        public IEnumerable<GameResult> GetAllGameResults()
        {
            return _databaseService.GetGameResults(DateTime.Now.AddYears(-10));
        }

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