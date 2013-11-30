<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="NietoYostenWebApp.CreateUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="CreateUserPanel" runat="server" DefaultButton="CreateUserWizard1$__CustomNav0$StepNextButtonButton">
    <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" ContinueDestinationPageUrl="~/Default.aspx"
        DisableCreatedUser="True" LoginCreatedUser="False" OnCreatedUser="CreateUserWizard1_CreatedUser" >
        <WizardSteps>
            <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                <ContentTemplate>
                    <table>
                        <tr>
                            <th colspan="2" align="left">Sign up to create your account:</th>
                        </tr>
                        <tr>
                            <td>Username:</td>
                            <td>
                                <asp:TextBox runat="server" ID="UserName" />
                                <asp:RequiredFieldValidator runat="server" ID="rfvUserName" ControlToValidate="UserName"
                                    ErrorMessage="Username is required." ValidationGroup="CreateUserWizard1"
                                    Display="Dynamic" />
                            </td>
                        </tr>
                        <tr>
                            <td>Password:</td>
                            <td>
                                <asp:TextBox runat="server" ID="Password" TextMode="Password" />
                                <asp:RequiredFieldValidator runat="server" ID="rfvPassword" ControlToValidate="Password"
                                    ErrorMessage="Password is required." ValidationGroup="CreateUserWizard1"
                                    Display="Dynamic" />
                            </td>
                        </tr>
                        <tr>
                            <td>Confirm Password:</td>
                            <td>
                                <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" />
                                <asp:RequiredFieldValidator runat="server" ID="rfvCofirmPassword" ControlToValidate="ConfirmPassword"
                                    ErrorMessage="Confirm Password is required." ValidationGroup="CreateUserWizard1"
                                    Display="Dynamic" />
                            </td>
                        </tr>
                        <tr>
                            <td>Email:</td>
                            <td>
                                <asp:TextBox runat="server" ID="Email" />
                                <asp:RequiredFieldValidator runat="server" ID="rfvEmail" ControlToValidate="Email"
                                    ErrorMessage="Email is required." Display="Dynamic" ValidationGroup="CreateUserWizard1" />
                                <br />
                                <asp:RegularExpressionValidator runat="server" ID="revEmail" ControlToValidate="Email"
                                    ErrorMessage="A valid email address is required." ValidationGroup="CreateUserWizard1"
                                    Display="Dynamic"
                                    ValidationExpression="^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                                    ControlToValidate="ConfirmPassword" Display="Dynamic"
                                    ErrorMessage="The Password and Confirmation Password must match."
                                    ValidationGroup="CreateUserWizard1">
                                </asp:CompareValidator><br />
                                <asp:RegularExpressionValidator runat="server" ID="revUserName" ControlToValidate="UserName"
                                    ErrorMessage="Username must be lowercase and contain only English characters and numbers." ValidationGroup="CreateUserWizard1"
                                    ValidationExpression="^[a-zA-Z0-9]+$" Display="Dynamic" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="true"></asp:Literal>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:CreateUserWizardStep>
            <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                <ContentTemplate>
                    <table>
                        <tr>
                            <td align="center" colspan="2">
                                Complete</td>
                        </tr>
                        <tr>
                            <td>
                                Your account has been successfully created and is waiting for approval. An email 
                                notification will be sent when the account is approved.</td>
                        </tr>
                        <tr>
                            <td align="right" colspan="2">
                                <asp:Button ID="ContinueButton" runat="server" CausesValidation="False" 
                                    CommandName="Continue" Text="Continue" ValidationGroup="CreateUserWizard1" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:CompleteWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
    </asp:Panel>
</asp:Content>
