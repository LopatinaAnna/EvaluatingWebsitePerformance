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
        public async Task<ActionResult> Results(string url)
        {
            if (string.IsNullOrEmpty(url) || url.Length > 1000)
            {
                return RedirectToAction("Index", new { status = "Incorrect input" });
            }

            try
            {
                await WebRequest.Create(url).GetResponseAsync();
            }
            catch
            {
                return RedirectToAction("Index", new { status = "No response" });
            }

            BaseRequest item;
            try
            {
                item = await service.AddBaseRequest(url, User.Identity.GetUserId());
            }
            catch (ValidationException exception)
            {
                return RedirectToAction("Index", new
                {
                    status = exception.Message
                });
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

            var baseRequest = await service.GetBaseRequests(requestId);

            var userId = User.Identity.GetUserId();

            if (baseRequest.UserId != userId)
            {
                return RedirectToAction("Index");
            }

            var baseRequestViewModel = new BaseRequestViewModel
            {
                BaseRequestUrl = baseRequest.BaseRequestUrl,
                SitemapRequests = baseRequest.SitemapRequests
            };

            return View(baseRequestViewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> HistoryList()
        {
            var userId = User.Identity.GetUserId();

            var userRequests = await service.GetBaseRequestsByUser(userId);

            var resultHistoryList = new List<HistoryViewModel>();

            userRequests.ForEach(c => resultHistoryList.Add(new HistoryViewModel
            {
                BaseRequestUrl = c.BaseRequestUrl,
                Creation = c.Creation
            }));

            return View(resultHistoryList);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> GetHistory(string url, DateTime creation)
        {
            var userId = User.Identity.GetUserId();

            if (url == null || creation == default)
            {
                return RedirectToAction("Index");
            }

            var requestId = await service.GetBaseRequestId(userId, url, creation);

            if (requestId > 0)
            {
                return RedirectToAction("HistoryResult", new { id = requestId });
            }
            else
            {
                return await HistoryList();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> RemoveHistoryResult(string baseRequestUrl, DateTime creation)
        {
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
    }
}