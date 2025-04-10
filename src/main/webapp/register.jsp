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
            // Check if the client has attempted to register under a username that already exists
            Object userExistsObj = session.getAttribute("userExists");
            Object incompleteAddress = session.getAttribute("incompleteAddress");
            if (userExistsObj != null) {
                // Display an error message if so
        %><p style="color: red;">This username is already in use! Please pick a different one.</p><%
            }
        %>

        <!-- TODO: JavaScript form input validation for postcode, email, phone number, etc-->
        <form action="register" method="post">
            <label for="username">Username:</label>
            <input type="text" id="username" name="username" required><br>
            <label for="password">Password:</label>
            <input type="password" id="password" name="password" required><br>
            <h3>Contact Details</h3>
            <label for="givenName">Name:</label>
            <input type="text" id="givenName" name="givenName" required><br>
            <label for="surname">Surname (Optional):</label>
            <input type="text" id="surname" name="surname"><br>
            <label for="phoneNumber">Phone Number (Optional):</label>
            <input type="tel" id="phoneNumber" name="phoneNumber"><br>
            <label for="email">Email:</label>
            <input type="email" id="email" name="email"><br>
            <h3>Address (Optional)</h3>
            <% if (incompleteAddress != null) {
                %><p style="color: red;">The address fields are incomplete! If you wish to add an address, please fill every field.</p><%
            }%>
            <label for="streetLine1">Street Line 1:</label>
            <input type="text" id="streetLine1" name="streetLine1"><br>
            <label for="streetLine2">Street Line 2:</label>
            <input type="text" id="streetLine2" name="streetLine2"><br>
            <label for="suburb">Suburb:</label>
            <input type="text" id="suburb" name="suburb"><br>
            <label for="state">State:</label>
            <select id="state" name="state">
                <option value="QLD">Queensland</option>
                <option value="NSW">New South Wales</option>
                <option value="ACT">Australian Capital Territory</option>
                <option value="VIC">Victoria</option>
                <option value="TAS">Tasmania</option>
                <option value="NT">Northern Territory</option>
                <option value="SA">South Australia</option>
                <option value="WA">Western Australia</option>
            </select><br>
            <label for="postcode">Post Code:</label>
            <input type="text" id="postcode" name="postcode"><br>
            <input type="submit" value="Register">
        </form>
        <a href="index.jsp">Go back</a>
    </body>
</html>
