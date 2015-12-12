@modeltype WebExModel

<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <meta charset="UTF-8">
    <title>
        WebEx @Html.RenderModule(Model, "Page", view:="title")
    </title>
    @For Each css In Html.RenderModulesViewOfType(Model, "css")
     @css
        next
</head>
<body>
    @RenderBody()
    @For Each corejs In Html.RenderModulesViewOfType(Model, "corejs", Function([module]) If([module].GetType().Name.ToLower().Contains("jquery"), -100, 0))
     @corejs 
        next
    @For Each js In Html.RenderModulesViewOfType(Model, "js")
     @js 
        next
</body>

</html>