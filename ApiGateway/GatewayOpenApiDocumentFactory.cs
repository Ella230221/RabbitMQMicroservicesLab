using System.Text.Json;

namespace ApiGateway;

public static class GatewayOpenApiDocumentFactory
{
    public static string CreateJson(HttpRequest request)
    {
        var serverUrl = $"{request.Scheme}://{request.Host.Value}";

        var document = new
        {
            openapi = "3.0.1",
            info = new
            {
                title = "ApiGateway",
                version = "v1",
                description = "Aggregated gateway contract for Products, Customers, Orders, and Payments."
            },
            servers = new[]
            {
                new { url = serverUrl }
            },
            paths = new Dictionary<string, object>
            {
                ["/products"] = new
                {
                    get = new
                    {
                        tags = new[] { "Products" },
                        summary = "Get all products",
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "Product list",
                                content = JsonContent("#/components/schemas/ProductDtoArray")
                            }
                        }
                    },
                    post = new
                    {
                        tags = new[] { "Products" },
                        summary = "Create a product",
                        requestBody = new
                        {
                            required = true,
                            content = JsonContent("#/components/schemas/CreateProductRequest")
                        },
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "Created product",
                                content = JsonContent("#/components/schemas/ProductDto")
                            }
                        }
                    }
                },
                ["/products/{id}"] = new
                {
                    get = new
                    {
                        tags = new[] { "Products" },
                        summary = "Get product by id",
                        parameters = new[]
                        {
                            new
                            {
                                name = "id",
                                @in = "path",
                                required = true,
                                schema = new { type = "integer", format = "int32" }
                            }
                        },
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "Product details",
                                content = JsonContent("#/components/schemas/ProductDto")
                            },
                            ["404"] = new { description = "Product not found" }
                        }
                    }
                },
                ["/customers"] = new
                {
                    get = new
                    {
                        tags = new[] { "Customers" },
                        summary = "Get all customers",
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "Customer list",
                                content = JsonContent("#/components/schemas/CustomerDtoArray")
                            }
                        }
                    },
                    post = new
                    {
                        tags = new[] { "Customers" },
                        summary = "Create a customer",
                        requestBody = new
                        {
                            required = true,
                            content = JsonContent("#/components/schemas/CreateCustomerRequest")
                        },
                        responses = new Dictionary<string, object>
                        {
                            ["201"] = new
                            {
                                description = "Created customer",
                                content = JsonContent("#/components/schemas/CustomerDto")
                            }
                        }
                    }
                },
                ["/customers/{id}"] = new
                {
                    get = new
                    {
                        tags = new[] { "Customers" },
                        summary = "Get customer by id",
                        parameters = new[]
                        {
                            new
                            {
                                name = "id",
                                @in = "path",
                                required = true,
                                schema = new { type = "integer", format = "int32" }
                            }
                        },
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "Customer details",
                                content = JsonContent("#/components/schemas/CustomerDto")
                            },
                            ["404"] = new { description = "Customer not found" }
                        }
                    }
                },
                ["/orders"] = new
                {
                    get = new
                    {
                        tags = new[] { "Orders" },
                        summary = "Get all orders",
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "Order list",
                                content = JsonContent("#/components/schemas/OrderDtoArray")
                            }
                        }
                    },
                    post = new
                    {
                        tags = new[] { "Orders" },
                        summary = "Create an order",
                        requestBody = new
                        {
                            required = true,
                            content = JsonContent("#/components/schemas/CreateOrderRequest")
                        },
                        responses = new Dictionary<string, object>
                        {
                            ["201"] = new
                            {
                                description = "Created order",
                                content = JsonContent("#/components/schemas/OrderDto")
                            },
                            ["400"] = new { description = "Validation or business rule failure" }
                        }
                    }
                },
                ["/orders/{id}"] = new
                {
                    get = new
                    {
                        tags = new[] { "Orders" },
                        summary = "Get order by id",
                        parameters = new[]
                        {
                            new
                            {
                                name = "id",
                                @in = "path",
                                required = true,
                                schema = new { type = "integer", format = "int32" }
                            }
                        },
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "Order details",
                                content = JsonContent("#/components/schemas/OrderDto")
                            },
                            ["404"] = new { description = "Order not found" }
                        }
                    }
                },
                ["/payments"] = new
                {
                    get = new
                    {
                        tags = new[] { "Payments" },
                        summary = "Get all processed payments",
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "Payment list",
                                content = JsonContent("#/components/schemas/PaymentDtoArray")
                            }
                        }
                    }
                },
                ["/payments/{id}"] = new
                {
                    get = new
                    {
                        tags = new[] { "Payments" },
                        summary = "Get payment by id",
                        parameters = new[]
                        {
                            new
                            {
                                name = "id",
                                @in = "path",
                                required = true,
                                schema = new { type = "integer", format = "int32" }
                            }
                        },
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "Payment details",
                                content = JsonContent("#/components/schemas/PaymentDto")
                            },
                            ["404"] = new { description = "Payment not found" }
                        }
                    }
                },
                ["/payments/by-order/{orderId}"] = new
                {
                    get = new
                    {
                        tags = new[] { "Payments" },
                        summary = "Get payment by order id",
                        parameters = new[]
                        {
                            new
                            {
                                name = "orderId",
                                @in = "path",
                                required = true,
                                schema = new { type = "integer", format = "int32" }
                            }
                        },
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "Payment for an order",
                                content = JsonContent("#/components/schemas/PaymentDto")
                            },
                            ["404"] = new { description = "Payment not found for order" }
                        }
                    }
                },
                ["/aggregations/orders/{id}"] = new
                {
                    get = new
                    {
                        tags = new[] { "Aggregations" },
                        summary = "Get aggregated order, customer, product, and payment details",
                        parameters = new[]
                        {
                            new
                            {
                                name = "id",
                                @in = "path",
                                required = true,
                                schema = new { type = "integer", format = "int32" }
                            }
                        },
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "Aggregated order details",
                                content = JsonContent("#/components/schemas/OrderAggregateDto")
                            },
                            ["404"] = new { description = "Order not found" }
                        }
                    }
                },
                ["/dashboard/summary"] = new
                {
                    get = new
                    {
                        tags = new[] { "Aggregations" },
                        summary = "Get dashboard summary aggregated across services",
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "Dashboard summary",
                                content = JsonContent("#/components/schemas/DashboardSummaryDto")
                            }
                        }
                    }
                }
            },
            components = new
            {
                schemas = new Dictionary<string, object>
                {
                    ["ProductDto"] = new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["id"] = new { type = "integer", format = "int32" },
                            ["name"] = new { type = "string" },
                            ["price"] = new { type = "number", format = "decimal" },
                            ["stock"] = new { type = "integer", format = "int32" }
                        }
                    },
                    ["ProductDtoArray"] = ArrayOf("#/components/schemas/ProductDto"),
                    ["CustomerDto"] = new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["id"] = new { type = "integer", format = "int32" },
                            ["name"] = new { type = "string" },
                            ["email"] = new { type = "string" }
                        }
                    },
                    ["CustomerDtoArray"] = ArrayOf("#/components/schemas/CustomerDto"),
                    ["CreateProductRequest"] = new
                    {
                        type = "object",
                        required = new[] { "name", "price", "stock" },
                        properties = new Dictionary<string, object>
                        {
                            ["name"] = new { type = "string" },
                            ["price"] = new { type = "number", format = "decimal" },
                            ["stock"] = new { type = "integer", format = "int32" }
                        }
                    },
                    ["CreateCustomerRequest"] = new
                    {
                        type = "object",
                        required = new[] { "name", "email" },
                        properties = new Dictionary<string, object>
                        {
                            ["name"] = new { type = "string" },
                            ["email"] = new { type = "string" }
                        }
                    },
                    ["CreateOrderRequest"] = new
                    {
                        type = "object",
                        required = new[] { "customerId", "productId", "quantity" },
                        properties = new Dictionary<string, object>
                        {
                            ["customerId"] = new { type = "integer", format = "int32" },
                            ["productId"] = new { type = "integer", format = "int32" },
                            ["quantity"] = new { type = "integer", format = "int32" }
                        }
                    },
                    ["OrderDto"] = new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["id"] = new { type = "integer", format = "int32" },
                            ["customerId"] = new { type = "integer", format = "int32" },
                            ["customerName"] = new { type = "string" },
                            ["productId"] = new { type = "integer", format = "int32" },
                            ["productName"] = new { type = "string" },
                            ["quantity"] = new { type = "integer", format = "int32" },
                            ["unitPrice"] = new { type = "number", format = "decimal" },
                            ["totalPrice"] = new { type = "number", format = "decimal" },
                            ["status"] = new { type = "string" },
                            ["createdAtUtc"] = new { type = "string", format = "date-time" }
                        }
                    },
                    ["OrderDtoArray"] = ArrayOf("#/components/schemas/OrderDto"),
                    ["PaymentDto"] = new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["id"] = new { type = "integer", format = "int32" },
                            ["orderId"] = new { type = "integer", format = "int32" },
                            ["customerId"] = new { type = "integer", format = "int32" },
                            ["amount"] = new { type = "number", format = "decimal" },
                            ["status"] = new { type = "string" },
                            ["processedAtUtc"] = new { type = "string", format = "date-time" }
                        }
                    },
                    ["PaymentDtoArray"] = ArrayOf("#/components/schemas/PaymentDto"),
                    ["OrderAggregateDto"] = new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["order"] = Ref("#/components/schemas/OrderDto"),
                            ["customer"] = Ref("#/components/schemas/CustomerDto"),
                            ["product"] = Ref("#/components/schemas/ProductDto"),
                            ["payment"] = Ref("#/components/schemas/PaymentDto")
                        }
                    },
                    ["DashboardSummaryDto"] = new
                    {
                        type = "object",
                        properties = new Dictionary<string, object>
                        {
                            ["productCount"] = new { type = "integer", format = "int32" },
                            ["customerCount"] = new { type = "integer", format = "int32" },
                            ["orderCount"] = new { type = "integer", format = "int32" },
                            ["paymentCount"] = new { type = "integer", format = "int32" },
                            ["totalRevenue"] = new { type = "number", format = "decimal" },
                            ["totalProcessedPayments"] = new { type = "number", format = "decimal" },
                            ["latestOrders"] = ArrayOf("#/components/schemas/OrderDto"),
                            ["latestPayments"] = ArrayOf("#/components/schemas/PaymentDto")
                        }
                    }
                }
            }
        };

        return JsonSerializer.Serialize(document, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    private static object JsonContent(string schemaRef) => new Dictionary<string, object>
    {
        ["application/json"] = new
        {
            schema = new Dictionary<string, object>
            {
                ["$ref"] = schemaRef
            }
        }
    };

    private static object ArrayOf(string schemaRef) => new
    {
        type = "array",
        items = new Dictionary<string, object>
        {
            ["$ref"] = schemaRef
        }
    };

    private static object Ref(string schemaRef) => new Dictionary<string, object>
    {
        ["$ref"] = schemaRef
    };
}
