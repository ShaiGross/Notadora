
SQLLOCALDB stop NotaDbServer
while ($true) {

    $info = SQLLOCALDB info NotaDbServer
    
    if (-not ($info -match "np")) {
    
        SQLLOCALDB start  NotaDbServer;
        $info = SQLLOCALDB info NotaDbServer
        $splitOutput = $info.Split(":");
        $srvName = $splitOutput[$splitOutput.Count - 2];        

        Write-Host "np:$srvName"
    }

    sleep 1;
}
