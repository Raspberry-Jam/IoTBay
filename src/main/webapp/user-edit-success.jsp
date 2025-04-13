<%@ page import="we.code.demo.model.entity.User" %><%--
  Created by IntelliJ IDEA.
  User: james
  Date: 11/4/25
  Time: 8:04 pm
  To change this template use File | Settings | File Templates.
--%>
<%@ page contentType="text/html;charset=UTF-8"%>

<%
  User user = (User) session.getAttribute("sessionUser");
%>
<html>
<head>
    <title>Successfully Updated "<%=user.getUsername()%></title>
</head>
<body>
    <h1>Successfully updated details for <%=user.getUsername()%></h1>
    <p><a href="index.jsp">Return home</a></p>
</body>
</html>
