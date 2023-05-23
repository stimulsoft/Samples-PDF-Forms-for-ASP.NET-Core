call npm i --force
call ng build --output-hashing none
copy dist\sti-forms-designer\*.* ..\wwwroot /Y
pause