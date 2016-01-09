
<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <meta charset="UTF-8">
    <title>
        WebEx @Html.RenderModule("PagesModule.PagesModule, App_Code", "title")
    </title>
    @Html.RenderModules("css")
</head>
<body>
    @RenderBody()
    @Html.RenderModules("js")
</body>

</html>