@echo off

if "%1"=="" (
    echo Usage: binplace.bat locationToRoslynBinaries
    goto exit
)

copy "%1\Roslyn.VisualStudio.Setup\Debug\net472\System.Collections.Immutable.dll" "Binaries\"
copy "%1\Roslyn.VisualStudio.Setup\Debug\net472\System.Composition.AttributedModel.dll" "Binaries\"
copy "%1\Roslyn.VisualStudio.Setup\Debug\net472\System.Composition.Convention.dll" "Binaries\"
copy "%1\Roslyn.VisualStudio.Setup\Debug\net472\System.Composition.Hosting.dll" "Binaries\"
copy "%1\Roslyn.VisualStudio.Setup\Debug\net472\System.Composition.Runtime.dll" "Binaries\"
copy "%1\Roslyn.VisualStudio.Setup\Debug\net472\System.Composition.TypedParts.dll" "Binaries\"
copy "%1\Roslyn.VisualStudio.Setup\Debug\net472\System.Reflection.Metadata.dll" "Binaries\"
copy "%1\Roslyn.VisualStudio.Setup\Debug\net472\System.Runtime.CompilerServices.Unsafe.dll" "Binaries\"

copy "%1\Microsoft.CodeAnalysis\Debug\netstandard2.0\Microsoft.CodeAnalysis.dll" "Binaries\"
copy "%1\Microsoft.CodeAnalysis\Debug\netstandard2.0\Microsoft.CodeAnalysis.xml" "Binaries\"

copy "%1\Microsoft.CodeAnalysis.Scripting\Debug\netstandard2.0\Microsoft.CodeAnalysis.Scripting.dll" "Binaries\"
copy "%1\Microsoft.CodeAnalysis.Scripting\Debug\netstandard2.0\Microsoft.CodeAnalysis.Scripting.xml" "Binaries\"

copy "%1\Microsoft.CodeAnalysis.Workspaces\Debug\netstandard2.0\Microsoft.CodeAnalysis.Workspaces.dll" "Binaries\"
copy "%1\Microsoft.CodeAnalysis.Workspaces\Debug\netstandard2.0\Microsoft.CodeAnalysis.Workspaces.xml" "Binaries\"

copy "%1\Microsoft.CodeAnalysis.CSharp\Debug\netstandard2.0\Microsoft.CodeAnalysis.CSharp.dll" "Binaries\"
copy "%1\Microsoft.CodeAnalysis.CSharp\Debug\netstandard2.0\Microsoft.CodeAnalysis.CSharp.xml" "Binaries\"

copy "%1\Microsoft.CodeAnalysis.CSharp.Scripting\Debug\netstandard2.0\Microsoft.CodeAnalysis.CSharp.Scripting.dll" "Binaries\"
copy "%1\Microsoft.CodeAnalysis.CSharp.Scripting\Debug\netstandard2.0\Microsoft.CodeAnalysis.CSharp.Scripting.xml" "Binaries\"

copy "%1\Microsoft.CodeAnalysis.CSharp.Workspaces\Debug\netstandard2.0\Microsoft.CodeAnalysis.CSharp.Workspaces.dll" "Binaries\"
copy "%1\Microsoft.CodeAnalysis.CSharp.Workspaces\Debug\netstandard2.0\Microsoft.CodeAnalysis.CSharp.Workspaces.xml" "Binaries\"

:exit