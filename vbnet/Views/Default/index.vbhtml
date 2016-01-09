
@Code
    Layout = "~/Views/_layout.vbhtml"
End code

Hello from WebEx!

@Html.RenderModule("Menu")
@Html.RenderModule("Pages")


@Html.RenderModule("partial", New With {.Param = "Hello from partial view"})