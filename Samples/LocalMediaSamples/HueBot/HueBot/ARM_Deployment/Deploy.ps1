$subscriptionName="ed43f29e-6445-4614-9829-d92b5ac46d51"
$resourceGroupName="tests"
$keyvaultName="andrueastman-keys"
$parameterFilePath="C:\Users\v-anomon\Documents\microsoft-graph-comms-samples\Samples\LocalMediaSamples\HueBot\HueBot\ARM_Deployment\AzureDeploy.Parameters.json"
$templateFilePath="C:\Users\v-anomon\Documents\microsoft-graph-comms-samples\Samples\LocalMediaSamples\HueBot\HueBot\ARM_Deployment\AzureDeploy.json"
$secretID="https://andrueastman-keys.vault.azure.net/secrets/truebot/82bc767199344ce7be5286b885a89640"

Connect-AzureRmAccount
Select-AzureRmSubscription -SubscriptionName $subscriptionName

Set-AzureRmKeyVaultAccessPolicy -VaultName $keyvaultName -EnabledForDeployment
New-AzureRmServiceFabricCluster -ResourceGroupName $resourceGroupName -SecretIdentifier $secretId -TemplateFile $templateFilePath -ParameterFile $parameterFilePath
