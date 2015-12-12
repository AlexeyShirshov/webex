@modeltype WebExModel

@Code
    Layout = "~/Views/_layout.vbhtml"
End code

Hello from WebEx!

@Html.RenderModule(Model, "Menu")
@Html.RenderModule(Model, "Page")
