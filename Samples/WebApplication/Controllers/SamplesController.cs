﻿using System;
using System.Collections.Generic;
using System.Net;
using App.Metrics;
using App.Metrics.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sillycore;
using Sillycore.BackgroundProcessing;
using Sillycore.Extensions;
using Sillycore.Web.Controllers;
using WebApplication.BackgroundJobs;
using WebApplication.Domain;
using WebApplication.HealthCheckers;

namespace WebApplication.Controllers
{
    //[Authorization("defaultPolicy")]
    [Route("samples")]
    public class SamplesController : SillyController
    {
        private readonly IMetrics _metrics;
        private readonly ILogger<SamplesController> _logger;
        private readonly IConfiguration _configuration;
        private readonly BackgroundJobManager _backgroundJobManager;

        public SamplesController(ILogger<SamplesController> logger, IConfiguration configuration, BackgroundJobManager backgroundJobManager, IMetrics metrics)
        {
            _logger = logger;
            _configuration = configuration;
            _backgroundJobManager = backgroundJobManager;
            _metrics = metrics;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(List<Sample>), (int)HttpStatusCode.OK)]
        public IActionResult QuerySamples(string name = null)
        {
            List<Sample> samples = new List<Sample>();
            _logger.LogDebug("QuerySamples called.");
            _metrics.Measure.Counter.Increment(MetricsRegistry.GlobalCounter, new MetricTags("MethodName", "QuerySamples"));
            using (_metrics.Measure.Timer.Time(MetricsRegistry.GlobalTimer, new MetricTags("MethodName", "QuerySamples")))
            {
                samples.Add(CreateNewSample());

            }
            return Ok(samples);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(Sample), (int)HttpStatusCode.Created)]
        public IActionResult CreateSample([FromBody]Sample request)
        {
            _metrics.Measure.Counter.Increment(MetricsRegistry.GlobalCounter, new MetricTags("MethodName", "CreateSample"));
            using (_metrics.Measure.Timer.Time(MetricsRegistry.GlobalTimer,
                new MetricTags("MethodName", "CreateSample")))
            {
                    return Created(request);
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok();
        }

        private Sample CreateNewSample()
        {
            return new Sample()
            {
                Id = Guid.NewGuid(),
                Name = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                CreatedOn = SillycoreApp.Instance.DateTimeProvider.Now
            };
        }
    }
}