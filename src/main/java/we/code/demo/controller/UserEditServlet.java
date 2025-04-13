package we.code.demo.controller;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import we.code.demo.model.entity.Address;
import we.code.demo.model.entity.Contact;
import we.code.demo.model.entity.User;

import java.io.IOException;

@WebServlet(name = "userEditServlet", value = "/userEdit")
public class UserEditServlet extends HttpServlet {
    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        req.getSession().setAttribute("incompleteAddress", null);

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

        // Check if we should update user fields
        boolean doPassword = password != null;
        boolean doContact = givenName != null;
        boolean doAddress = false;

        // Check if the state string is a valid enum type
        try {
            Address.State.valueOf(stateString);
        } catch (IllegalArgumentException e) {
            resp.sendRedirect("/error.jsp");
            return;
        }

        // Check if any of the strings are empty
        if (streetLine1.isEmpty() || streetLine2.isEmpty() || suburb.isEmpty() || postcode.isEmpty()) {
            // Check if any of the other strings have values (if so, invalid address)
            if (!streetLine1.isEmpty() || !streetLine2.isEmpty() || !suburb.isEmpty() || !postcode.isEmpty()) {
                req.getSession().setAttribute("incompleteAddress", true);
                req.getRequestDispatcher("/register.jsp").forward(req, resp);
                return;
            }
        } else {
            doAddress = true;
        }

        Contact contact = new Contact(givenName, surname, phoneNumber, email);
        Address address = doAddress ? new Address(streetLine1, streetLine2, suburb, Address.State.valueOf(stateString), postcode) : null;

        User user = (User) req.getSession().getAttribute("sessionUser");

        if (doPassword) user.setPassword(password);
        if (doContact) user.setContact(contact);
        if (doAddress) user.setAddress(address);

        resp.sendRedirect("/user-edit-success.jsp");
    }
}
