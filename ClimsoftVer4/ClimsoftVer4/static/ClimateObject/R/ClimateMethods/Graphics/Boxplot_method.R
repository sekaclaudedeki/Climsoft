climate$methods(Boxplot = function(data_list= list(), fill_col="blue",interest_var,factor=month_label,
                                   whisker_col="red", convert=TRUE,data_period_label=daily_label,
                                   title="Rain Amount boxplot",whisklty=1,xlab="Months",
                                   ylab=interest_var,horizontal=FALSE){
  #--------------------------------------------------------------------------------------------#
  # This function plots the boxplot for the variable of interest i.e interest_var
  # To Do: how to handle threshold    
  #-------------------------------------------------------------------------------------------#
  
  # Specifying the needed variable
  data_list = add_to_data_info_required_variable_list( data_list, list(interest_var))
  #Using convert_data
  data_list=c(data_list,convert_data=convert)
  # Specifying the data_time_period
  data_list=add_to_data_info_time_period( data_list, data_period_label)
  # use data_list to get the required data objects
  climate_data_objs = get_climate_data_objects( data_list )   
  
  for( data_obj in climate_data_objs){
    #threshold = data_obj$get_meta_new(threshold_label,missing(threshold),threshold)
    data_name = data_obj$get_meta(data_name_label)
    
    # add month column if not present
    if( !( data_obj$is_present(month_label)) ) {
      data_obj$add_year_month_day_cols()
    }
    
    interest_col=data_obj$getvname(interest_var)
    
    # Get the title of the column of factors
    
    factor_col = data_obj$getvname(factor)
    # Access data in methods
    curr_data_list = data_obj$get_data_for_analysis(data_list)
    
    for( curr_data in curr_data_list ) {
       
      #print(curr_data[[factor_col]])
      # Draw the boxplot
      boxplot(as.formula(paste(interest_col,"~",factor_col)), data=curr_data, whiskcol=whisker_col,col=fill_col, xlab=xlab,ylab=ylab,
              main= c( data_name, title), whisklty=whisklty,horizontal=horizontal)
    } 
  }
}
)

#the factor variable is month in this method. There is a need to check on how to handle the factors
#when geting climate_data_objs

