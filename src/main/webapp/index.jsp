<%@ page contentType="text/html; charset=UTF-8" pageEncoding="UTF-8" %>

<%
    String sessionToken = (String) session.getAttribute("sessionToken");
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
        <%if (sessionToken != null) {
            %><h2><a href="welcome.jsp">Go to Dashboard</a></h2><%
        }%>
    </body>
</html>