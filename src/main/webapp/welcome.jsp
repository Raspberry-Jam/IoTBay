<%--
  Created by IntelliJ IDEA.
  User: james
  Date: 3/4/25
  Time: 1:47 am
  To change this template use File | Settings | File Templates.
--%>
<%@ page import="we.code.demo.model.entity.User" %>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<%
    User user = (User) session.getAttribute("sessionUser");
%>
<html>
    <head>
        <title>Welcome <%=user.getUsername()%></title>
    </head>
    <body>
        <h1>Welcome, <%=user.getUsername()%>!</h1>
        <p>Your session token: <%=user.getSessionToken()%></p>
        <a href="index.jsp">Go back</a>
    </body>
</html>
