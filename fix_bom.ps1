$path = "d:\Projects\BVTL-X\WebApp\Views\BaoCaoQuyCH07\Index.cshtml"
$text = [System.IO.File]::ReadAllText($path)
$utf8BOM = New-Object System.Text.UTF8Encoding $true
[System.IO.File]::WriteAllText($path, $text, $utf8BOM)
