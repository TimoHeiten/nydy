# helpers
check_exit_code() {
    if [ $? -ne 0 ]; then
        echo "----------$1 failed----------"
        exit 1
    fi
}

# build phase
echo "Building nydy"
dotnet clean
dotnet build 
check_exit_code "Build"

# echo linting and style analysis
# todo editor config / linting / style cop / code analysis
# dotnet format
# linting

echo "Running Tests"
# tests 
cd tests/heitech.nydy.tests
dotnet test
check_exit_code "Tests"

# echo packaging and push to nuget
# publish phase

if [ "$1" == "push" ]; then
    echo "Publishing nydy to nuget"
    cd ../../src/heitech.nydy
    dotnet pack -c Release
    check_exit_code "Pack"
    dotnet nuget push bin/Release/*.nupkg --source https://api.nuget.org/v3/index.json --api-key $2
    check_exit_code "Push"
fi