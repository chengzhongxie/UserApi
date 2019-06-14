using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace User.API.Filters
{
    /// <summary>
    /// 错误异常处理
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(IHostingEnvironment env, ILogger<GlobalExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // 判断是否是项目中已知的错误信息
            var json = new JsonErrorResponse();
            if (context.Exception.GetType() == typeof(UserOperationException))
            {
                json.Message = context.Exception.Message;
                if (_env.IsDevelopment())// 判断是开发环境还是生产环境
                {
                    json.DeveloperMessage = context.Exception.StackTrace;// 开发环境将堆栈信息返回
                }
                context.Result = new BadRequestObjectResult(json);
            }
            else
            {
                if (_env.IsDevelopment())// 判断是开发环境还是生产环境
                {
                    json.Message = context.Exception.Message;
                    json.DeveloperMessage = context.Exception.StackTrace;// 开发环境将堆栈信息返回
                }
                else
                {
                    json.Message = "发生了未知内部错误";
                }
                context.Result = new InternalServerErrorObjectResult(json);
            }
            _logger.LogError(context.Exception, context.Exception.Message);
            context.ExceptionHandled = true;
        }
    }

    /// <summary>
    /// 定义未知错误返回信息
    /// </summary>
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error) : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

}
