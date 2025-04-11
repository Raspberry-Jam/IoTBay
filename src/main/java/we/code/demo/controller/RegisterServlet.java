package we.code.demo.controller;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import we.code.demo.model.entity.Address;
import we.code.demo.model.entity.Contact;
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
        req.getSession().setAttribute("userExists", null);
        req.getSession().setAttribute("incompleteAddress", null);

        // Get the current in-memory list of users
        List<User> users = UserDataAccessObject.getUsers();

        // Pull the user attributes from the request data
        String username = req.getParameter("username");
        String password = req.getParameter("password");
        String givenName = req.getParameter("givenName");
        String surname = req.getParameter("surname");
        String phoneNumber = req.getParameter("phoneNumber");
        String email = req.getParameter("email");
        String streetLine1 = req.getParameter("streetLine1");
        String streetLine2 = req.getParameter("streetLine2");
        String suburb = req.getParameter("suburb");
        String postcode = req.getParameter("postcode");
        String stateString = req.getParameter("state");

        boolean doAddress = false;

        // Check if the request is malformed, and send client to error page if it is
        if (username == null || password == null || givenName == null) {
            System.err.println("Malformed request, missing required fields");
            resp.sendRedirect("/error.jsp");
            return;
        }

        // Check if the stateString is a legal value
        if (stateString != null) {
            try {
                Address.State.valueOf(stateString);
            } catch (IllegalArgumentException e) {
                //e.printStackTrace();
                resp.sendRedirect("/error.jsp");
                return;
            }
        }

        // Check if the user has an incomplete Address section
        int validAddress = validateAddress(streetLine1, streetLine2, suburb, stateString, postcode);
        if (validAddress == 0) doAddress = true;
        if (validAddress == 2) {
            req.getSession().setAttribute("incompleteAddress", true);
            req.getRequestDispatcher("/register.jsp").forward(req, resp);
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

        // Construct user data into POJOs (plain old java objects) for serialisation and storage
        Contact contact = new Contact(givenName, surname, phoneNumber, email);
        Address address = doAddress ? new Address(streetLine1, streetLine2, suburb, Address.State.valueOf(stateString), postcode) : null;
        User newUser = new User(username, password, contact, address);

        // Add the new User object to the persistent in-memory storage
        UserDataAccessObject.addUser(newUser);

        // Send the client to the register landing page
        req.getSession().setAttribute("username", username);
        resp.sendRedirect("thanks-register.jsp");
    }

    // TODO: Make this better. This is awful and I hate it.
    private int validateAddress(String streetLine1, String streetLine2, String suburb, String state, String postcode) {
        // 0 = Valid Address
        // 1 = Empty Address
        // 2 = Invalid Address

        // Check if the state string is a valid enum type
        try {
            Address.State.valueOf(state);
        } catch (IllegalArgumentException e) {
            return 2;
        }

        // Check if any of the strings are empty
        if (streetLine1.isEmpty() || streetLine2.isEmpty() || suburb.isEmpty() || postcode.isEmpty()) {
            // Check if any of the other strings have values (if so, invalid address)
            if (!streetLine1.isEmpty() || !streetLine2.isEmpty() || !suburb.isEmpty() || !postcode.isEmpty()) {
                return 2;
            }
            return 1;
        }
        return 0;
    }
}
