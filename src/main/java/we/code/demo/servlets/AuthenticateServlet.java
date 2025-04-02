package we.code.demo.servlets;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

import java.io.IOException;
import java.time.LocalDateTime;

@WebServlet(name = "authenticateServlet", value = "/authenticate")
public class AuthenticateServlet extends HttpServlet {
    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        req.setAttribute("badLogin", null);

        String username = req.getParameter("username");
        String password = req.getParameter("password");

        if (username != null && password != null) {
            if (username.equals("admin") && password.equals("password")) {
                String sessionToken = java.util.UUID.randomUUID().toString();
                req.getSession().setAttribute("sessionToken", sessionToken);
                resp.sendRedirect("welcome.jsp");
            } else {
                req.getSession().setAttribute("badLogin", true);
                req.getRequestDispatcher("index.jsp").forward(req, resp);
            }
        } else {
            req.getRequestDispatcher("/error.jsp").forward(req, resp);
        }
    }
}
