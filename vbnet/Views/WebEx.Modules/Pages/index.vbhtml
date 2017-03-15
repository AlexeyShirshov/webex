@modeltype PagesModule.Page

@*<p>*@
    @Model.Title
@*</p>*@

<p>Plugins</p>
<ul>
    @Html.RenderModulesFolder("Plugins")
</ul>