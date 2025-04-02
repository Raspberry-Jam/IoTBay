<%@ page contentType="text/html; charset=UTF-8" pageEncoding="UTF-8" %>
<!DOCTYPE html>
<html>
    <head>
        <title>JSP - Hello World</title>
    </head>
    <body>
        <h1>Please log in</h1>
        <%
            Object badLoginObj = session.getAttribute("badLogin");
            if (badLoginObj != null) {
                boolean badLogin = (boolean) badLoginObj;
                if (badLogin) {
                    %><p style="color: red;">Incorrect login details. Please try again.</p><%
                }
            }
        %>
        <form action="authenticate" method="post">
            <label for="username">Username:</label><br>
            <input type="text" id="username" name="username" required><br>
            <label for="password">Password:</label><br>
            <input type="password" id="password" name="password" required><br>
            <input type="submit" value="Log In">
        </form>
    </body>
</html>