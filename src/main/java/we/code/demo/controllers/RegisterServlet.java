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

@WebServlet(name = "registerNewUser", value = "/register")
public class RegisterServlet extends HttpServlet {
    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        req.setAttribute("userExists", null);

        List<User> users = UserDataAccessObject.getUsers();

        String username = req.getParameter("username");
        String password = req.getParameter("password");

        if (username == null || password == null) {
            resp.sendRedirect("/error.jsp");
            return;
        }

        for (User user : users) {
            if (username.equals(user.getUsername())) {
                req.getSession().setAttribute("userExists", true);
                req.getRequestDispatcher("/register.jsp").forward(req, resp);
                return;
            }
        }

        req.getSession().setAttribute("badLogin", null);

        User newUser = new User(username, password);

        UserDataAccessObject.saveUser(newUser);

        resp.sendRedirect("thanks-register.jsp");
    }
}
