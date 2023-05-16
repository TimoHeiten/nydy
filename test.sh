cd test/heitech.nydy.Tests/

# if args exists use filter
if [ $1 != "" ]; then
    dotnet test --filter "FullyQualifiedName~heitech.nydy.Tests.$1"
else
    dotnet test
fi 
