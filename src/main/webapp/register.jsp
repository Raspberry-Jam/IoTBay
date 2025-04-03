<%--
  Created by IntelliJ IDEA.
  User: james
  Date: 3/4/25
  Time: 6:24 pm
  To change this template use File | Settings | File Templates.
--%>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<html>
    <head>
        <title>Register new user</title>
    </head>
    <body>
        <h1>Register new user</h1>
        <%
            Object userExistsObj = session.getAttribute("userExists");
            if (userExistsObj != null) {
        %><p style="color: red;">This username is already in use! Please pick a different one.</p><%
            }
        %>
        <form action="register" method="post">
            <label for="username">Username:</label><br>
            <input type="text" id="username" name="username" required><br>
            <label for="password">Password:</label><br>
            <input type="password" id="password" name="password" required><br>
            <input type="submit" value="Register">
        </form>
        <a href="index.jsp">Go back</a>
    </body>
</html>
