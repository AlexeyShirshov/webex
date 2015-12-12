
Imports System.Collections.Generic
Imports System.Dynamic
Imports System.Linq
Imports System.Text

Partial Public Class WebExModel
    Inherits Westwind.Utilities.Dynamic.Expando
    'Public Const webexPluginsRegistry As String = "webex:PluginsRegistry"

    <AttributeUsage(AttributeTargets.Method)> _
    Public Class LoadModelAttribute
        Inherits Attribute

    End Class
    <AttributeUsage(AttributeTargets.[Property])> _
    Public Class ModuleAttribute
        Inherits Attribute

    End Class
    Public Sub Load(ParamArray args As Object())
        Dim methods2call = From method In Me.[GetType]().GetMethods() Where IsLoad(method) Select method

        For Each method In methods2call
            Dim methodParams = method.GetParameters()
            If methodParams.Count() = 0 Then
                method.Invoke(Me, Nothing)
            ElseIf args IsNot Nothing AndAlso args.Count() > 0 Then
                If methodParams.Count() <= args.Count() Then
                    Dim params2Call = New Object(methodParams.Count() - 1) {}
                    Array.Copy(args, params2Call, methodParams.Count())
                    method.Invoke(Me, params2Call)
                End If
            End If
        Next

        Dim plugins = AppConfig.PluginsRegistry 'TryCast(System.Web.HttpContext.Current.Application(webexPluginsRegistry), Plugins)
        If plugins IsNot Nothing And plugins.ExternalModules IsNot Nothing Then
            For Each [module] In plugins.ExternalModules
                [module].Load(Me, args)
            Next
        End If
    End Sub

    Private Function IsLoad(method As System.Reflection.MethodInfo) As Boolean
        Return method.GetCustomAttributes(GetType(LoadModelAttribute), False).Any()
    End Function

    Public Function TryGetProperty(name As String, ByRef result As Object) As Boolean
        If Not Properties.TryGetValue(name, result) Then
            Return GetProperty(Me, name, result)
        End If

        Return True
    End Function

    Public Iterator Function GetMultiViewModules() As IEnumerable(Of IModule)
        For Each [module] In (From p In [GetType]().GetProperties(System.Reflection.BindingFlags.[Public] Or System.Reflection.BindingFlags.GetProperty Or System.Reflection.BindingFlags.Instance)
                               Where GetType(IModule).IsAssignableFrom(p.GetMethod.ReturnType)
                               Select m = TryCast(p.GetMethod.Invoke(Me, Nothing), IModule)
                               Where m IsNot Nothing
                               Select m).Union(From p In Properties
                                               Select m = TryCast(p.Value, IModule)
                                               Where m IsNot Nothing
                                               Select m)
            Yield [module]
        Next
    End Function

End Class
