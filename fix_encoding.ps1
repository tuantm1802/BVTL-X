$path = "d:\Projects\BVTL-X\WebApp\Views\BaoCaoQuyCH07\Index.cshtml"
$text = [System.IO.File]::ReadAllText($path)
[System.IO.File]::WriteAllText($path, $text, [System.Text.Encoding]::UTF8)
