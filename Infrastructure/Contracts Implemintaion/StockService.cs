using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;

using AutoMapper;

using Domain.Entites;

namespace Infrastructure.Contracts_Implemintaion;

    public class StockService: IStockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StockService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> AddStockAsync(StockTransactionDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null) return false;
            product.ChangeStock(dto.Quantity, product.StockQuantity,dto.TransactionType);

            var stockRecord = new StockTransaction(product.Id, dto.Quantity, dto.TransactionType);

             _unitOfWork.Products.Update(product);
            await _unitOfWork.StockTransactions.AddAsync(stockRecord);

            return await _unitOfWork.CommitAsync() > 0;
        }
    }

