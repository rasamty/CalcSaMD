output "dev_default_host_name" {
  value       = module.dev.default_host_name
  description = "DEV's own *.azurestaticapps.net address - works the moment terraform apply finishes, no domain or DNS needed."
}

output "qa_default_host_name" {
  value = module.qa.default_host_name
}

output "uat_default_host_name" {
  value = module.uat.default_host_name
}

output "prod_default_host_name" {
  value = module.prod.default_host_name
}

output "dev_api_key" {
  value       = module.dev.api_key
  description = "Phase 7 puts this into the DEV_AZURE_STATIC_WEB_APPS_API_TOKEN GitHub Secret."
  sensitive   = true
}

output "qa_api_key" {
  value     = module.qa.api_key
  sensitive = true
}

output "uat_api_key" {
  value     = module.uat.api_key
  sensitive = true
}

output "prod_api_key" {
  value     = module.prod.api_key
  sensitive = true
}
