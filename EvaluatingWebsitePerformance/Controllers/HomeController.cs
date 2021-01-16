using EvaluatingWebsitePerformance.BusinessLogic.Interfaces;
using EvaluatingWebsitePerformance.Data.Entities;
using EvaluatingWebsitePerformance.Infrastructure;
using EvaluatingWebsitePerformance.Infrastructure.Filters;
using EvaluatingWebsitePerformance.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EvaluatingWebsitePerformance.Controllers
{
    [ActionException]
    public class HomeController : Controller
    {
        private readonly IService service;

        public HomeController(IService service)
        {
            this.service = service;
        }

        [HttpGet]
        public ActionResult Index(string status)
        {
            ViewData["status-message"] = status;
            return View();
        }

        [HttpGet]
        public ActionResult Results(BaseRequestViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Results(CreateBaseRequestModel model)
        {
            var isValid = ValidateUrl(model.BaseRequestUrl);

            if (!isValid.Item1)
            {
                return RedirectToAction("Index", new { status = isValid.Item2 });
            }

            BaseRequest item;
            model.UserId = User.Identity.GetUserId();

            try
            {
                item = await service.AddBaseRequest(model);
            }
            catch (ValidationException exception)
            {
                return RedirectToAction("Index", new { status = exception.Message });
            }

            var resultModel = new BaseRequestViewModel
            {
                BaseRequestUrl = item.BaseRequestUrl,
                SitemapRequests = item.SitemapRequests
                .OrderBy(c => c.MinResponseTime)
                .ToList()
            };

            return Results(resultModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> HistoryResult(int? id)
        {
            var requestId = id.GetValueOrDefault();

            if (requestId <= 0)
            {
                return RedirectToAction("Index");
            }

            var baseRequest = await service.GetBaseRequest(requestId);

            if (baseRequest == null || baseRequest.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }

            var baseRequestViewModel = new BaseRequestViewModel
            {
                BaseRequestUrl = baseRequest.BaseRequestUrl,
                SitemapRequests = baseRequest.SitemapRequests
                .OrderBy(c => c.MinResponseTime)
                .ToList()
            };

            return View(baseRequestViewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> HistoryList()
        {
            var userRequests = await service.GetBaseRequestsByUser(User.Identity.GetUserId());

            var resultHistoryList = new List<HistoryViewModel>();

            userRequests.ForEach(c => resultHistoryList.Add(new HistoryViewModel
            {
                BaseRequestUrl = c.BaseRequestUrl,
                Creation = c.Creation
            }));

            return View(resultHistoryList.OrderByDescending(c => c.Creation));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> GetHistory(string url, DateTime creation)
        {
            if (url == null || creation == default)
            {
                return RedirectToAction("Index");
            }

            var userId = User.Identity.GetUserId();

            int requestId;

            try
            {
                requestId = await service.GetBaseRequestId(userId, url, creation);
            }
            catch (Exception)
            {
                return RedirectToAction("HistoryList");
            }

            return RedirectToAction("HistoryResult", new { id = requestId });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> RemoveHistoryResult(string baseRequestUrl, DateTime creation)
        {
            if (baseRequestUrl == null || creation == default)
            {
                return RedirectToAction("Index");
            }

            var userId = User.Identity.GetUserId();

            await service.DeleteBaseRequest(userId, baseRequestUrl, creation);

            return RedirectToAction("HistoryList");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> RemoveAllHistory()
        {
            var userId = User.Identity.GetUserId();

            await service.DeleteAllBaseRequest(userId);

            return RedirectToAction("HistoryList");
        }

        private (bool, string) ValidateUrl(string url)
        {
            if (string.IsNullOrEmpty(url) || url.Length > 1000)
            {
                return (false, "Incorrect input");
            }

            try
            {
                new Uri(url);
            }
            catch
            {
                return (false, "Incorrect url");
            }

            try
            {
                WebRequest.Create(url).GetResponse();
            }
            catch
            {
                return (false, "No response");
            }

            return (true, "Url is correct");
        }
    }
}