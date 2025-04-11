package we.code.demo.controller;

import jakarta.servlet.*;
import jakarta.servlet.annotation.WebFilter;
import jakarta.servlet.http.Cookie;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import we.code.demo.model.dao.UserDataAccessObject;
import we.code.demo.model.entity.User;

import java.io.IOException;
import java.util.List;

// TODO: Change session state to be decoupled from user login, allowing anonymous users to retain application state
@WebFilter("/*")
public class SessionCookieFilter implements Filter {
    @Override
    public void doFilter(ServletRequest servletRequest, ServletResponse servletResponse, FilterChain filterChain) throws IOException, ServletException {
        HttpServletRequest req = (HttpServletRequest) servletRequest;
        HttpServletResponse resp = (HttpServletResponse) servletResponse;
        doSessionCookieCheck(req, resp, filterChain);
    }

    private void doSessionCookieCheck(HttpServletRequest req, HttpServletResponse resp, FilterChain chain) throws ServletException, IOException {
        Object checkedCookies = req.getSession().getAttribute("checkedCookies");
        if (checkedCookies != null) {
            chain.doFilter(req, resp);
            return;
        }

        req.getSession().setAttribute("checkedCookies", true);
        Cookie[] cookies = req.getCookies();
        Cookie sessionCookie = null;

        for (Cookie cookie : cookies) {
            if (cookie.getName().equals("sessionToken")) {
                sessionCookie = cookie;
                break;
            }
        }

        if (sessionCookie == null) {
            chain.doFilter(req, resp);
            return;
        }

        List<User> users = UserDataAccessObject.getUsers();
        User user = null;

        for (User u : users) {
            if (u.getSessionToken().toString().equals(sessionCookie.getValue())) {
                user = u;
            }
        }

        if (user == null) return;

        if (user.getSessionExpiry().isBefore(java.time.LocalDateTime.now())) {
            sessionCookie.setMaxAge(0);
            resp.addCookie(sessionCookie);
            return;
        }

        req.getSession().setAttribute("sessionToken", user.getSessionToken());
        chain.doFilter(req, resp);
    }
}