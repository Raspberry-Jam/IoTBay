package we.code.demo.controller;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import we.code.demo.model.entity.User;
import we.code.demo.model.dao.UserDataAccessObject;

import java.io.IOException;
import java.util.List;

// Create an API endpoint for registering new users
@WebServlet(name = "registerNewUser", value = "/register")
public class RegisterServlet extends HttpServlet {
    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        // If the session had a userExists triggered previously, and comes back to this page later,
        // it will incorrectly display an error message that is not relevant anymore.
        req.setAttribute("userExists", null);

        // Get the current in-memory list of users
        List<User> users = UserDataAccessObject.getUsers();

        // Pull the user attributes from the request data
        String username = req.getParameter("username");
        String password = req.getParameter("password");

        // Check if the request is malformed, and send client to error page if it is
        if (username == null || password == null) {
            resp.sendRedirect("/error.jsp");
            return;
        }

        // Check if the username is already being used, and inform the client if it is
        for (User user : users) {
            if (username.equals(user.getUsername())) {
                req.getSession().setAttribute("userExists", true);
                req.getRequestDispatcher("/register.jsp").forward(req, resp);
                return;
            }
        }

        // Clear the badLogin attribute, in case it is currently set, before the user goes
        // to the login page through the thanks-register.jsp page.
        req.getSession().setAttribute("badLogin", null);

        // Create a new User object with the validated data
        User newUser = new User(username, password);

        // Add the new User object to the persistent in-memory storage
        UserDataAccessObject.saveUser(newUser);

        // Send the client to the register landing page
        resp.sendRedirect("thanks-register.jsp");
    }
}
