<%--
  Created by IntelliJ IDEA.
  User: james
  Date: 3/4/25
  Time: 6:34 pm
  To change this template use File | Settings | File Templates.
--%>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<html>
  <head>
      <title>Log In</title>
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
