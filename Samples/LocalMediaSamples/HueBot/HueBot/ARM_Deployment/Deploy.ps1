$subscriptionName="ed43f29e-6445-4614-9829-d92b5ac46d51"
$resourceGroupName="tests"
$keyvaultName="bots"
$parameterFilePath="C:\Users\v-anomon\Documents\microsoft-graph-comms-samples\Samples\LocalMediaSamples\HueBot\HueBot\ARM_Deployment\AzureDeploy.Parameters.json"
$templateFilePath="C:\Users\v-anomon\Documents\microsoft-graph-comms-samples\Samples\LocalMediaSamples\HueBot\HueBot\ARM_Deployment\AzureDeploy.json"
$secretID="https://bots.vault.azure.net/secrets/test-certificate/379eed1339414de98f0da88c0a1f2064"

Connect-AzureRmAccount
Select-AzureRmSubscription -SubscriptionName $subscriptionName

Set-AzureRmKeyVaultAccessPolicy -VaultName $keyvaultName -EnabledForDeployment
New-AzureRmServiceFabricCluster -ResourceGroupName $resourceGroupName -SecretIdentifier $secretId -TemplateFile $templateFilePath -ParameterFile $parameterFilePath
