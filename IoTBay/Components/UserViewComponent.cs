using IoTBay.Models.DTOs;
using IoTBay.Utils;
using Microsoft.AspNetCore.Mvc;

namespace IoTBay.Components;

public class UserViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var currentUser = SessionUtils.GetObjectFromJson<UserSessionDto>(HttpContext.Session, "currentUser");
        return View(currentUser);
    }
}