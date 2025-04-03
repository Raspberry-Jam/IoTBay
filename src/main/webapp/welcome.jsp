<%--
  Created by IntelliJ IDEA.
  User: james
  Date: 3/4/25
  Time: 1:47 am
  To change this template use File | Settings | File Templates.
--%>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<%
    String username = (String) session.getAttribute("username");
    String token = (String) session.getAttribute("sessionToken");
%>
<html>
    <head>
        <title>Welcome <%=username%></title>
    </head>
    <body>
        <h1>Welcome, <%=username%>!</h1>
        <p>Your session token: <%=token%></p>
        <a href="index.jsp">Go back</a>
    </body>
</html>
