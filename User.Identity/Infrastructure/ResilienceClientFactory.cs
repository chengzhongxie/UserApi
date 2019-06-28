﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Resilience;
using Resilience.RequestProvider;

namespace User.Identity.Infrastructure
{
    public class ResilienceClientFactory
    {
        private ILogger<ResilienceClientFactory> _logger;
        private IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// 重试次数
        /// </summary>
        private int _retryCount;
        /// <summary>
        /// 熔断之前允许的异常次数
        /// </summary>
        private int _exceptionCountAllowedBeforeBreaking;

   

        public ResilienceClientFactory(ILogger<ResilienceClientFactory> logger, IHttpContextAccessor httpContextAccessor, int retryCount, int exceptionCountAllowedBeforeBreaking)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _retryCount = retryCount;
            _exceptionCountAllowedBeforeBreaking = exceptionCountAllowedBeforeBreaking;
        }

        private Policy[] CreatePolicy(string origin)
        {
            return new Policy[]
            {
                Policy.Handle<HttpRequestException>()
                .WaitAndRetry(
                    _retryCount,
                    retryAttempt=>TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)),
                    (exception,timeSpan,retryCount,context)=>{
                    var msg=$"第 {retryCount} 次重试"+$"or {context.PolicyKey}"+$" due to: {exception}";
                    _logger.LogWarning(msg);
                    _logger.LogDebug(msg);
                }),
                Policy.Handle<HttpRequestException>()
                .CircuitBreaker(
                    _exceptionCountAllowedBeforeBreaking,
                    TimeSpan.FromMilliseconds(1),
                    (excpeiton, duration) =>
                    {
                        _logger.LogTrace("熔断器打开了");
                    },()=>{
                    _logger.LogTrace("熔断器关闭");
                    }
                    )
            };
        }
    }
}
