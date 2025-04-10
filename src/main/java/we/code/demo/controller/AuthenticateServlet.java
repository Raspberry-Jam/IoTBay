package we.code.demo.controller;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import we.code.demo.model.entity.User;
import we.code.demo.model.UserDataAccessObject;

import java.io.IOException;
import java.util.List;

// Create an API endpoint for handling user authentication
@WebServlet(name = "authenticateServlet", value = "/authenticate")
public class AuthenticateServlet extends HttpServlet {
    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        // If the session has had a badLogin triggered previously, and comes from register
        // it would incorrectly display the login error message. This prevents that.
        req.setAttribute("badLogin", null);

        // Get the current in-memory list of users
        List<User> users = UserDataAccessObject.getUsers();

        // Pull the user attributes from the request data
        String username = req.getParameter("username");
        String password = req.getParameter("password");

        // TODO: Database integration to check for real usernames and real passwords

        // Check that the request contains the required user data for checking login
        if (username != null && password != null) {
            for (User user : users) { // Loop over all users in the in-memory data store
                if (username.equals(user.getUsername()) && password.equals(user.getPassword())) { // Check if the details match
                    String sessionToken = java.util.UUID.randomUUID().toString(); // Create a unique session token
                    // TODO: Store session token in browser cookie
                    req.getSession().setAttribute("sessionToken", sessionToken); // Pass the session token along to the client
                    resp.sendRedirect("welcome.jsp"); // Send the client to the landing page
                    return;
                }
            }
            // No matches were made in the data store, meaning some of the details were wrong.
            req.getSession().setAttribute("badLogin", true); // Inform the client that it sent incorrect details
            resp.sendRedirect("login.jsp"); // Send the response to the client
        } else { // The request was malformed.
            req.getRequestDispatcher("error.jsp").forward(req, resp); // Send the client to an error page
        }
    }
}
