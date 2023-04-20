using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Filters
{
	public class OnActionExecutingFilter : ActionFilterAttribute
	{
		//private readonly UserService _userService;

		//public OnActionExecutingFilter(UserService userService)
		//{
		//	_userService = userService;
		//}

		//public override void OnActionExecuting(ActionExecutingContext context)
		//{
		//	if (context.HttpContext.Request.Path.Value.Contains("api"))
		//	{
		//		PostRequest request = (PostRequest)context.ActionArguments["postRequest"];

		//		string tryToHack = Shared.Helpers.TextHelper.CheckObjectForHarmfulCodes(ref request);

		//		if (!string.IsNullOrEmpty(tryToHack))
		//			context.Result = new ContentResult { Content = "INFO_Harmful_Code", StatusCode = 403 };
		//		//   return HandleResult(_workspaceService.ConvertToIActionResult(new WorkspaceResponse { Error = new Shared.AppError() { ErrorCode = 2, ErrorMessage = "INFO_Warmful_Code", MethodName = MethodInfo.GetCurrentMethod().Name, SourceLayer = "ML" } }));   

		//		if (!context.HttpContext.Request.Path.Value.Contains("api/main") &&
		//			!context.HttpContext.Request.Path.Value.Contains("login"))
		//		{

		//			if (!_userService.IsAuthenticatedUser(request?.LoginAuth?.AccessToken))
		//				context.Result = new ContentResult { Content = "INFO_No_Authenticated", StatusCode = 403 };
		//			// return HandleResult(_workspaceService.ConvertToIActionResult(new WorkspaceResponse { Error = new Shared.AppError() { ErrorCode = 2, ErrorMessage = "INFO_No_Authenticated", MethodName = MethodInfo.GetCurrentMethod().Name, SourceLayer = "ML" } }));


		//		}
		//	}
		//}
	}
}
