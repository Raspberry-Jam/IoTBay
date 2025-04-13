<%@ page import="we.code.demo.model.entity.User" %>
<%@ page contentType="text/html; charset=UTF-8" pageEncoding="UTF-8" %>

<%
    User user = (User) session.getAttribute("sessionUser");
%>

<!DOCTYPE html>
<html>
    <head>
        <title>Hello World</title>
    </head>
    <body>
        <h1>Welcome!</h1>
        <h2><a href="login.jsp">Log In</a></h2>
        <h2><a href="register.jsp">Register</a></h2>
        <%if (user != null) {
            %>
            <h2><a href="welcome.jsp">Go to Dashboard</a></h2>
            <h2><a href="user-edit.jsp">Edit User</a></h2>
            <h2><a href="logout.jsp">Log Out</a></h2>
            <%

        }%>
    </body>
</html>