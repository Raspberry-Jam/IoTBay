<%--
  Created by IntelliJ IDEA.
  User: james
  Date: 13/4/25
  Time: 12:41 pm
  To change this template use File | Settings | File Templates.
--%>
<%@ page contentType="text/html;charset=UTF-8"%>
<%@ page import="we.code.demo.model.entity.User"%>
<%
    User user = (User) session.getAttribute("sessionUser");
    if (user == null) {
        response.sendRedirect("index.jsp");
    }
    assert user != null;
%>
<html>
<head>
    <title>Logout</title>
</head>
<body>
    <h1><%=user.getUsername()%>, would you like to log out?</h1>
    <p><a href="logout">Log Out</a></p><br>
    <p><a href="index.jsp">Go back to home</a></p>
</body>
</html>
