@modeltype IEnumerable(of MenuItem)

@if Model isnot nothing then
    @<ul class="nav nav-tabs">
        @for each menu in Model        
            @<li><a href="@menu.Url">@menu.Name</a></li>
        next
    </ul>
end if