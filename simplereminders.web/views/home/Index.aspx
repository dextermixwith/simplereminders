<%@ page title="" language="C#" masterpagefile="~/Views/Shared/Site.Master" inherits="System.Web.Mvc.ViewPage<simplereminders.web.controllers.viewmodels.IndexViewModel>" %>

<asp:content id="Content1" contentplaceholderid="TitleContent" runat="server">
    Simple Reminders
</asp:content>
<asp:content id="Content2" contentplaceholderid="MainContent" runat="server">
    <h2>Welcome to Simple Reminders</h2>
    <% if (!string.IsNullOrEmpty(Model.Info)) { %>
        <div class="summary-result-container">
            <ul class="summary-result">
                <li><%= Model.Info %></li>
            </ul>
        </div>
    <% } %>
    <% Html.BeginForm(); %>
        <div class="toolbar">
            <label for="timePeriod"> Show appointments for:</label>
            <select id="timePeriod" name="timePeriod">
                <option value="" selected="selected">All Calendar</option>
                <option value="today">Today</option>
                <option value="tomorrow">Tomorrow</option>
                <option value="thisweek">This Week</option>
                <option value="thismonth">This Month</option>
            </select>
            <input type="submit" id="updateTimePeriod" name="updateTimePeriod" value="Update List" title="Update List of Appointments for this period" />
        </div>
    <% Html.EndForm(); %>
    
    <div class="tableWrapper" style="clear:both;">
        <table class="sortableTable clickableTable" style="padding:5px;">
            <col style="width: 200px" />
            <col style="width: 75px" />
            <col style="width: 150px" />
            <col style="width: 475px" />
            <col style="width: 75px" />
            <tr>
                <th>
                    Start Date/Time
                </th>
                <th>
                    Duration (minutes)
                </th>
                <th>
                    Description
                </th>
                <th>
                    Attendees
                </th>
                <th>
                    Action
                </th>
            </tr>
            <% foreach (var appointment in Model.AppointmentList)
               { %>
            <tr>
                <td>
                    <%= appointment.StartingAt %>
                </td>
                <td>
                    <%= appointment.Duration %>
                </td>
                <td>
                    <%= appointment.Description %>
                </td>
                <td>
                <ul>
                    <%foreach (var attendee in appointment.Attendees) { %>
                        <li style="font-weight:bold;<%=(attendee.ResponseStatus == "confirmed" ? "color:green;" : (attendee.ResponseStatus == "cancelled" ? "color:red;" : string.Empty))%>"><%=attendee.Email %> <%= string.IsNullOrEmpty(attendee.ResponseStatus) ? string.Empty : string.Format("({0})", attendee.ResponseStatus.Replace("noreponse", "no response"))  %></li>
                    <% } %>
                </ul>
                </td>
                <td>
                    <%if (appointment.Attendees.Any(a => a.ResponseStatus == null)) {	%>
                        <% Html.BeginForm("SendRemindersForEvent", "Home");%>
                        <%= Html.Hidden("eventId", appointment.EventId) %>
                        <input type="submit" value="Send Reminders" />
                        <% Html.EndForm(); %>
                    <%} %>
                </td>
            </tr>
            <%
                } %>
        </table>
    </div>
</asp:content>
