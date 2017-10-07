$rootPath = "D:\Projects\GitHub\MicroservicesSample"

# Customers
# WebApi Port 5001 
$cdProjectDir = "cd /d $rootPath\src\Customers";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 200

# Customers.PhoneLineOrderCompletedSubscriber
# Process
$cdProjectDir = "cd /d $rootPath\src\Customers.PhoneLineOrderCompletedSubscriber";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 200

# PhoneLineOrderer
# WebApi Port 5002 
$cdProjectDir = "cd /d $rootPath\src\PhoneLineOrderer";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 200

# PhoneLineOrderer.OrdersPlacedSubscriber
# Process
$cdProjectDir = "cd /d $rootPath\src\PhoneLineOrders.OrdersPlacedSubscriber";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 200

# FakeBt
# WebApi Port 5003 
$cdProjectDir = "cd /d $rootPath\src\FakeBt";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;
Start-Sleep -Milliseconds 200

# FakeBt.OrderUpdater
# Process
$cdProjectDir = "cd /d $rootPath\src\FakeBt.OrderUpdater";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;

# SmsSender.Subscribers
# Process
$cdProjectDir = "cd /d $rootPath\src\SmsSender.Subscribers";
$params=@("/C"; $cdProjectDir; " && dotnet run"; )
Start-Process -Verb runas "cmd.exe" $params;