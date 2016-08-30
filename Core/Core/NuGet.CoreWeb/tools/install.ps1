param($installPath, $toolsPath, $package, $project)
$project.Object.References.Add("System.DirectoryServices")|out-null
$project.Object.References.Add("System.DirectoryServices.AccountManagement")|out-null
