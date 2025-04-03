package we.code.demo.controllers;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import we.code.demo.models.User;
import we.code.demo.models.UserDataAccessObject;

import java.io.IOException;
import java.util.List;

@WebServlet(name = "authenticateServlet", value = "/authenticate")
public class AuthenticateServlet extends HttpServlet {
    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        req.setAttribute("badLogin", null);

        List<User> users = UserDataAccessObject.getUsers();

        String username = req.getParameter("username");
        String password = req.getParameter("password");

        // TODO: Database integration to check for real usernames and real passwords
        if (username != null && password != null) {
            for (User user : users) {
                if (username.equals(user.getUsername()) && password.equals(user.getPassword())) {
                    String sessionToken = java.util.UUID.randomUUID().toString();
                    req.getSession().setAttribute("sessionToken", sessionToken);
                    resp.sendRedirect("welcome.jsp");
                    return;
                }
            }
            req.getSession().setAttribute("badLogin", true);
            req.getRequestDispatcher("/login.jsp").forward(req, resp);
        } else {
            req.getRequestDispatcher("/error.jsp").forward(req, resp);
        }
    }
}
