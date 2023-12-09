
namespace OpenSilverCodeGenerator
{
    internal class Settings
    {
        public string ApiKey { get; set; }

        public string ApiModel { get; set; } = "gpt-4-1106-preview";

        public int MaxTokens { get; set; } = 4000;

        public int MaxAttempts { get; set; } = 3;

        public string Setup { get; set; } = @"
You are the developer of an OpenSilver application.
I need to create a simple user control inside the Grid according to the description.
I will send you a description. You return me only XAML code according to my description.
Do not use any events in the xaml.
Use different colors and gradients to make the component more attractive.";

        public string Examples { get; set; } = @"
An empty TextBox
-----
<Grid><TextBox></TextBox></Grid>
-----
Login Form
-----
<Grid>
    <TextBlock Height=""23"" HorizontalAlignment=""Left"" Margin=""10,10,0,0"" Name=""LoginHeading"" Text=""Login:"" VerticalAlignment=""Top"" FontSize=""17"" FontStretch=""ExtraCondensed""/>
    <TextBlock Height=""50"" HorizontalAlignment=""Left"" Margin=""24,48,0,0"" Name=""textBlockHeading"" VerticalAlignment=""Top"" FontSize=""12"" FontStyle=""Italic"" Padding=""5"">
        Note: Please login here to view the features of this site. If you are new on this site then <LineBreak /><!--line break-->
        please click on
        <!--textblock as a Hyperlink.-->
        <TextBlock>
                <Hyperlink FontSize=""14"" FontStyle=""Normal"">Register</Hyperlink>
        </TextBlock>
        <!--end textblock as a hyperlink-->
        button
    </TextBlock>
    <TextBlock Height=""23"" HorizontalAlignment=""Left"" Margin=""66,120,0,0"" Name=""textBlock1"" Text=""Email"" VerticalAlignment=""Top"" Width=""67"" />
    <TextBlock Height=""23"" HorizontalAlignment=""Left"" Margin=""58,168,0,0"" Name=""textBlock2"" Text=""Password"" VerticalAlignment=""Top"" Width=""77"" />
    <TextBox Height=""23"" HorizontalAlignment=""Left"" Margin=""118,115,0,0"" Name=""textBoxEmail"" VerticalAlignment=""Top"" Width=""247"" />
    <PasswordBox Height=""23"" HorizontalAlignment=""Left"" Margin=""118,168,0,0"" Name=""passwordBox1"" VerticalAlignment=""Top"" Width=""247"" />
    <Button Content=""Login"" Height=""23"" HorizontalAlignment=""Left"" Margin=""118,211,0,0"" Name=""button1"" VerticalAlignment=""Top"" Width=""104""/>
    <TextBlock Height=""23"" HorizontalAlignment=""Left"" x:Name =""errormessage"" VerticalAlignment=""Top"" Width=""247"" Margin=""118,253,0,0""  OpacityMask=""Crimson"" Foreground=""#FFE5572C""  />
</Grid>
";
    }
}
