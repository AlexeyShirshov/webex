
<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <meta charset="UTF-8">
    <title>
        WebEx @Html.RenderModule("PagesModule.PagesModule, App_Code", view:="title")
    </title>
    @For Each css In Html.RenderModulesViewOfType("css")
     @css
        next
</head>
<body>
    @RenderBody()
    @For Each js In Html.RenderModulesViewOfType("js")
     @js 
        next
</body>

</html>