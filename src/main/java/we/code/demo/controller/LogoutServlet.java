package we.code.demo.controller;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.Cookie;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import we.code.demo.model.entity.User;

import java.io.IOException;
import java.time.LocalDateTime;

@WebServlet(name = "logoutServlet", value = "/logout")
public class LogoutServlet extends HttpServlet {
    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        logout(req, resp);
    }

    @Override
    protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        logout(req, resp);
    }

    protected void logout(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        User user = (User) req.getSession().getAttribute("sessionUser");
        if (user == null) { // Bad logout request
            resp.sendRedirect("/error.jsp");
            return;
        }

        // Invalidate user session on server
        user.setSessionExpiry(LocalDateTime.now());
        user.setSessionToken(null);

        // Invalidate user session for browser
        req.getSession().setAttribute("sessionUser", null);

        // Invalidate user session token in cookies
        for (Cookie cookie : req.getCookies()) {
            if (cookie.getName().equals("sessionToken")) {
                cookie.setMaxAge(0);
            }
        }

        // Send user to home page
        resp.sendRedirect("/index.jsp");
    }
}
