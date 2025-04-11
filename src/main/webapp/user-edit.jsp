<%--
  Created by IntelliJ IDEA.
  User: james
  Date: 11/4/25
  Time: 6:49 pm
  To change this template use File | Settings | File Templates.
--%>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<%@ page import="we.code.demo.model.entity.User" %>
<%@ page import="we.code.demo.model.entity.Address" %>

<%
    User user = (User) session.getAttribute("sessionUser");
    Object incompleteAddress = request.getAttribute("incompleteAddress");
%>
<html>
<head>
    <title>Edit User "<%=user.getUsername()%></title>
</head>
<body>
    <h1>Edit details for <%=user.getUsername()%></h1>
    <form action="editUser" method="post">
        <h3>Change Password</h3>
        <label for="password">Password:</label>
        <input type="password" id="password" name="password"><br>
        <h3>Change Contact Details</h3>
        <label for="givenName">Given Name:</label>
        <input type="text" id="givenName" name="givenName" value="<%=user.getContact().getGivenName()%>"><br>
        <label for="surname">Surname:</label>
        <input type="text" id="surname" name="surname" value="<%=user.getContact().getSurname()%>"><br>
        <label for="phoneNumber">Phone Number:</label>
        <input type="tel" id="phoneNumber" name="phoneNumber" value="<%=user.getContact().getPhoneNumber()%>"><br>
        <label for="email">Email:</label>
        <input type="email" id="email" name="email" value="<%=user.getContact().getPhoneNumber()%>"><br>
        <h3>Change Address</h3>
        <% if (incompleteAddress != null) {
        %><p style="color: red;">The address fields are incomplete! If you wish to add an address, please fill every field.</p><%
        }%>
        <%
            if (user.getAddress() != null) {
                %>
                <label for="streetLine1">Street Line 1:</label>
                <input type="text" id="streetLine1" name="streetLine1" value="<%=user.getAddress().getStreetLine1()%>"><br>
                <label for="streetLine2">Street Line 2:</label>
                <input type="text" id="streetLine2" name="streetLine2" value="<%=user.getAddress().getStreetLine2()%>"><br>
                <label for="suburb">Suburb:</label>
                <input type="text" id="suburb" name="suburb" value="<%=user.getAddress().getSuburb()%>"><br>
                <label for="state">State:</label>
                <select id="state" name="state">
                    <%
                        String s = user.getAddress().getState().toString();
                    %>
                    <option value="QLD" <%if(s.equals("QLD")){%>selected<%}%>>Queensland</option>
                    <option value="NSW" <%if(s.equals("NSW")){%>selected<%}%>>New South Wales</option>
                    <option value="ACT" <%if(s.equals("ACT")){%>selected<%}%>>Australian Capital Territory</option>
                    <option value="VIC" <%if(s.equals("VIC")){%>selected<%}%>>Victoria</option>
                    <option value="TAS" <%if(s.equals("TAS")){%>selected<%}%>>Tasmania</option>
                    <option value="NT" <%if(s.equals("NT")){%>selected<%}%>>Northern Territory</option>
                    <option value="SA" <%if(s.equals("SA")){%>selected<%}%>>South Australia</option>
                    <option value="WA" <%if(s.equals("WA")){%>selected<%}%>>Western Australia</option>
                </select><br>
                <%
            } else {
                %>
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
                <%
            }
        %>
        <label for="postcode">Post Code:</label>
        <input type="text" id="postcode" name="postcode"><br>
        <input type="submit" value="Update">
    </form>
</body>
</html>
